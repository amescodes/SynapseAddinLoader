using System.Linq;
using System.Windows;

using Synapse;

namespace SynapseAddinLoader.Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static SynapseClient client;

        protected override void OnStartup(StartupEventArgs e)
        {
            //base.OnStartup(e);
            client = SynapseClient.StartSynapseClient();
            string windowTitle = string.Join(" ", e.Args);

            MainWindow window = new MainWindow(windowTitle);
            window.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            client?.Shutdown();
        }

        /// <summary>
        /// Returns a JSON string.
        /// </summary>
        /// <param name="methodId"></param>
        /// <param name="inputParametersAsStrings"></param>
        /// <returns></returns>
        public static string TryDoRevit(string methodId, params object[] inputParametersAsStrings)
        {
            return client.TryDoRevit<object>(methodId, out _, inputParametersAsStrings);
        }

        public static T TryDoRevit<T>(string methodId, out string jsonResponse, params object[] inputParametersAsStrings)
        {
            jsonResponse = client.TryDoRevit(methodId, out T responseOutput, inputParametersAsStrings);
            return responseOutput;
        }
    }
}
