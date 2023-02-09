using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Build.Construction;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Utilities.Collections;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

class Build : NukeBuild
{
    [Solution]
    readonly Solution Solution;

    //[GitVersion]
    //readonly GitVersion GitVersion;

    static readonly bool IsReleaseBuild = false;

    [Parameter("Configuration to build. Use the Revit version year (2021, 2022, 2023) as a parameter to only build for one version.")]
    readonly Configuration Configuration = IsReleaseBuild ? Configuration.Release : Configuration.Debug;

    [Parameter("Revit version to build for debug.")]
    readonly string RevitVersion;

    public Guid ClientId => new Guid("29A04CA9-9A79-41BF-9268-5D3A293A605C");
    public string AppName => "Synapse Addin Loader";
    public string ExternalAppClassName => $"{Solution.Name}.ExtApp"; //? potential to automate

    public string RevitVendorId => "AMES_CODES";
    public string RevitVendorDescription => "ames.codes";

    public string AddinFileContents => $@"<?xml version=""1.0"" encoding=""utf-8""?>
<RevitAddIns>
  <AddIn Type=""Application"">
    <FullClassName>{ExternalAppClassName}</FullClassName>
    <ClientId>{ClientId}</ClientId>
    <Name>{AppName}</Name>
    <Assembly>RELATIVE_DLL_LOCATION</Assembly>
    <VendorId>{RevitVendorId}</VendorId>
    <VendorDescription>{RevitVendorDescription}</VendorDescription>
  </AddIn>
</RevitAddIns>";

    string addinFileString => $"{Solution.Name}.addin";

    public static int Main() => Execute<Build>(x => x.CompileAll);

    Target Compile2021 => _ => _
        .DependsOn(CompileClient)
        .Executes(() =>
        {
            CompileByVersion("2021");
        });

    Target Debug2021 => _ => _
        .DependsOn(Compile2021)
        .DependsOn(CopyToAddinDirectory);

    Target Compile2022 => _ => _
        .DependsOn(CompileClient)
        .Executes(() =>
        {
            CompileByVersion("2022");
        });

    Target Debug2022 => _ => _
        .DependsOn(Compile2022)
        .DependsOn(CopyToAddinDirectory);

    Target Compile2023 => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            CompileByVersion("2023");
        });

    Target Debug2023 => _ => _
        .DependsOn(Compile2023)
        .DependsOn(CopyToAddinDirectory);

    Target CompileClient => _ => _
        .Executes(() =>
        {
            Project clientProject = Solution.GetProject($"{Solution.Name}.Client");
            string clientBuildDirPath = $"{clientProject.Directory}/bin/{Configuration}";

            DotNetBuild(_ => _
                .SetProjectFile(clientProject)
                .SetConfiguration(Configuration)
                .SetFramework(clientProject.GetProperty("TargetFramework"))
                .SetRuntime("win10-x64")
                .SetSelfContained(false)
                .SetOutputDirectory(clientBuildDirPath));
        });

    Target CopyToAddinDirectory => _ => _
        .Executes(() =>
        {
            // if RevitVersion isn't null,
            // it came from the command line
            if (RevitVersion != null)
            {
                CopyToPcAddinsPath(RevitVersion);

                return;
            }

            // 2021
            CopyToPcAddinsPath("2021");
            // 2022
            CopyToPcAddinsPath("2022");
            // 2023
            CopyToPcAddinsPath("2023");
        });

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            Solution.Directory.GlobDirectories("**/bin", "**/obj").Where(p => p.Parent?.Name != "build").ForEach(DeleteDirectory);

            string commonPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            string pcAddinPath = Path.Combine(commonPath, $@"Autodesk\\Revit\Addins");

            // 2021
            string pcAddinPath2021 = Path.Combine(pcAddinPath, $@"2021\\{Solution.Name}");
            if (Directory.Exists(pcAddinPath2021))
            {
                Directory.Delete(pcAddinPath2021, true);

                string addinFile2021 = pcAddinPath2021.Append(".addin");
                if (File.Exists(addinFile2021))
                {
                    File.Delete(addinFile2021);
                }
            }
            // 2022
            string pcAddinPath2022 = Path.Combine(pcAddinPath, $@"2022\\{Solution.Name}");
            if (Directory.Exists(pcAddinPath2022))
            {
                Directory.Delete(pcAddinPath2022, true);

                string addinFile2022 = pcAddinPath2022.Append(".addin");
                if (File.Exists(addinFile2022))
                {
                    File.Delete(addinFile2022);
                }
            }
            // 2023
            string pcAddinPath2023 = Path.Combine(pcAddinPath, $@"2023\\{Solution.Name}");
            if (Directory.Exists(pcAddinPath2023))
            {
                Directory.Delete(pcAddinPath2023, true);

                string addinFile2023 = pcAddinPath2023.Append(".addin");
                if (File.Exists(addinFile2023))
                {
                    File.Delete(addinFile2023);
                }
            }
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetRestore();
        });

    Target CompileAll => _ => _
        .DependsOn(Compile2021)
        .DependsOn(Compile2022)
        .DependsOn(Compile2023);

    void CompileByVersion(string revitVersion)
    {
        Project ipcClientProject = Solution.GetProject($"{Solution.Name}.Client");
        string clientBuildDirPath = $"{ipcClientProject.Directory}/bin/{Configuration}";

        if (!Directory.Exists(clientBuildDirPath))
        {
            throw new Exception("other dll directory doesn't exist");
        }

        BuildRevitProject(revitVersion);
        AbsolutePath versionProjectPath = GetBuildDirectoryForRevitProject($"Revit{revitVersion}");

        string nestedDllDirPath = Path.Combine(versionProjectPath, Solution.Name, "Client");
        if (!Directory.Exists(nestedDllDirPath))
        {
            Directory.CreateDirectory(nestedDllDirPath);
        }

        CopyFilesRecursively(clientBuildDirPath, nestedDllDirPath);

        CreateAddinManifest(versionProjectPath);
    }

    /// <summary>
    /// Returns path of built DLL(s).
    /// </summary>
    /// <param name="revitVersion"></param>
    /// <returns></returns>
    void BuildRevitProject(string revitVersion)
    {
        Project revitProject = Solution.GetProject($@"Revit{revitVersion}");
        string buildDirPath = Path.Combine(revitProject.Directory, "bin", Configuration, Solution.Name);

        DotNetBuild(_ => _
            .SetProjectFile(revitProject)
            .SetConfiguration(Configuration)
            .SetFramework(revitProject.GetProperty("TargetFramework"))
            .SetOutputDirectory(buildDirPath));

    }

    void CopyToPcAddinsPath(string revitVersion)
    {
        string commonPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
        string pcAddinPath = Path.Combine(commonPath, $@"Autodesk\\Revit\Addins");
        // 2023
        string buildPath = GetBuildDirectoryForRevitProject($"Revit{revitVersion}");
        if (Directory.Exists(buildPath))
        {
            string pcAddinPathByVersion = Path.Combine(pcAddinPath, $"{revitVersion}");
            CopyFilesRecursively(buildPath, pcAddinPathByVersion);
        }
    }

    void CreateAddinManifest(string outputDirectory)
    {
        string assemblyName = Solution.Name;
        string relativeDllLocation = IsReleaseBuild ? $"{assemblyName}.dll" : $"{assemblyName}/{assemblyName}.dll";
        string finalAddinContents = AddinFileContents.Replace("RELATIVE_DLL_LOCATION", relativeDllLocation);
        
        File.WriteAllText(Path.Combine(outputDirectory, addinFileString), finalAddinContents);
    }

    AbsolutePath GetBuildDirectoryForRevitProject(string projectName)
    {
        IReadOnlyCollection<string> buildDir = GlobDirectories(Solution.Directory, $"{projectName}/bin/{Configuration}");
        return (AbsolutePath)buildDir.MaxBy(p => p.Length);
    }

    static void CopyFilesRecursively(string sourcePath, string targetPath)
    {
        //Now Create all of the directories
        foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
        {
            Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
        }

        //Copy all the files & Replaces any files with the same name
        foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
        {
            File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
        }
    }
}