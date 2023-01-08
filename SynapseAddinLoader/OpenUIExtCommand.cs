using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Interop;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using Synapse.Revit;
using Synapse;

using SynapseAddinLoader.UI;


namespace SynapseAddinLoader
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class OpenUIExtCommand : IExternalCommand
    {
        private static MainWindow synapseAddinLoaderWindow;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            if (synapseAddinLoaderWindow == null ||
               !synapseAddinLoaderWindow.IsVisible)
            {
                SynapseRevitService.Initialize();

                string assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

                synapseAddinLoaderWindow = new MainWindow()
                {
                    DataContext = new MainWindowViewModel($"Synapse Addin Loader v{assemblyVersion}")
                };
                _ = new WindowInteropHelper(synapseAddinLoaderWindow) { Owner = commandData.Application.MainWindowHandle };

                synapseAddinLoaderWindow.Show();
            }
            else
            {
                synapseAddinLoaderWindow.Activate();
                synapseAddinLoaderWindow.Focus();
            }

            return Result.Succeeded;
        }
    }
}
