using System.Reflection;
using System.Windows.Interop;

using SynapseAddinLoader.Core;
using SynapseAddinLoader.UI;

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Revit.Async;
using Synapse.Revit;

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
                UIApplication uiapp = commandData.Application;
                App.UiApplication = uiapp;

                SynapseRevitService.Initialize(uiapp);
                RevitTask.Initialize(uiapp);

                string assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

                synapseAddinLoaderWindow = new MainWindow()
                {
                    DataContext = new MainWindowViewModel($"Synapse Addin Loader v{assemblyVersion}")
                };
                _ = new WindowInteropHelper(synapseAddinLoaderWindow) { Owner = uiapp.MainWindowHandle };

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
