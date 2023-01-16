using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.UI;
using Autodesk.Windows;
using RibbonPanel = Autodesk.Revit.UI.RibbonPanel;

namespace SynapseAddinLoader
{
    public class ExtApp : IExternalApplication
    {
        public static UIApplication UiApplication { get; set; }

        public Result OnStartup(UIControlledApplication application)
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            string assemblyPath = executingAssembly.Location;
            string assemblyNamespace = executingAssembly.GetName().Name;

            RibbonPanel debuggerPanel = application.CreateRibbonPanel("Synapse");
            RibbonItemData btnData = new PushButtonData("synapseDebugBtn", "Load", assemblyPath, $@"{assemblyNamespace}.{nameof(OpenUIExtCommand)}");
            debuggerPanel.AddItem(btnData);

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }
}
