using System.Reflection;
using System.Windows.Interop;

using SynapseAddinLoader.Core;

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Synapse.Revit.Service;

namespace SynapseAddinLoader
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class OpenUIExtCommand : IExternalCommand
    {
        private static SynapseProcess process;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            SynapseRevitService.Initialize(uiapp);

            if (process != null &&
                process.IsOpen())
            {
                process.ActivateProcess();
                return Result.Succeeded;
            }

            // make command runner dictionary for translating grpc messages to revit actions
            process = SynapseRevitService.ConnectSynapse(new AddinLoaderRevitSynapse());

            string assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString(3);
            process.Start("Synapse Addin Loader v" + assemblyVersion);

            uiapp.ApplicationClosing += Application_ApplicationClosing;

            return Result.Succeeded;
        }

        private void Application_ApplicationClosing(object sender, Autodesk.Revit.UI.Events.ApplicationClosingEventArgs e)
        {
            process?.Close();

            UIControlledApplication uiapp = sender as UIControlledApplication;
            if (uiapp == null)
            {
                return;
            }

            uiapp.ApplicationClosing -= Application_ApplicationClosing;
        }
    }
}
