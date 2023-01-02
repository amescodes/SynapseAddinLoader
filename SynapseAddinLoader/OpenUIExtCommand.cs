using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using Synapse.Revit;

using Revit.Async;
using Synapse;
using SynapseAddinLoader.UI;

namespace SynapseAddinLoader
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class OpenUIExtCommand : IExternalCommand
    {
        private MainWindow synapseAddinLoaderWindow;
        private SynapseClient synapseClient;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            if (synapseAddinLoaderWindow == null ||
               !synapseAddinLoaderWindow.IsVisible)
            {
                SynapseRevitService.Initialize();
                RevitTask.Initialize(commandData.Application);

                string assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

                synapseClient = SynapseClient.StartSynapseClient();
                synapseAddinLoaderWindow = new MainWindow()
                {
                    DataContext = new MainWindowViewModel($"Synapse Addin Loader v{assemblyVersion}", synapseClient)
                };
                synapseAddinLoaderWindow.Closing += SynapseAddinLoaderWindowOnClosing;

                synapseAddinLoaderWindow.Show();
            }
            else
            {
                synapseAddinLoaderWindow.Activate();
                synapseAddinLoaderWindow.Focus();
            }

            return Result.Succeeded;
        }

        private void SynapseAddinLoaderWindowOnClosing(object sender, CancelEventArgs e)
        {
            synapseClient.Shutdown();
            synapseAddinLoaderWindow.Closing -= SynapseAddinLoaderWindowOnClosing;
        }
    }
}
