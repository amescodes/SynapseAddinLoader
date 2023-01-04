using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SynapseAddinLoader.Core
{
    public class ResolveAssembly
    {
        private string assemblyDirectory;

        public ResolveAssembly(string assemblyDirectory)
        {
            this.assemblyDirectory = assemblyDirectory;
        }

        public void AddResolveAssemblyHandler()
        {
            AppDomain.CurrentDomain.AssemblyResolve += Resolve;
        }

        public void RemoveResolveAssemblyHandler()
        {
            AppDomain.CurrentDomain.AssemblyResolve -= Resolve;
        }
        
        public Assembly LoadFromPath(string filePath)
        {
            Assembly result = null;
            try
            {
                Monitor.Enter(this);
                result = Assembly.LoadFile(filePath);
            }
            finally
            {
                Monitor.Exit(this);
            }

            return result;
        }

        private Assembly Resolve(object sender, ResolveEventArgs args)
        {
            string filePath = FindAssemblyPath(args.Name);

            if (File.Exists(filePath))
            {
                return LoadFromPath(filePath);
            }
            else
            {
                return null;
            }
        }

        private string FindAssemblyPath(string assembly)
        {
            string assemblyName = assembly.Substring(0, assembly.IndexOf(','));

            string sourceDir = Path.GetDirectoryName(assemblyDirectory);
            string filePath = Path.Combine(sourceDir, $"{assemblyName}.dll");
            if (!Directory.Exists(sourceDir) ||
                !File.Exists(filePath))
            {
                return string.Empty;
            }

            return filePath;
        }

    }
}
