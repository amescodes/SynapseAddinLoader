using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Synapse;

using Autodesk.Revit.UI;

namespace SynapseAddinLoader.Core
{
    public class App
    {
        private static SynapseClient client;

        public static UIApplication UiApplication { get; set; }

        /// <summary>
        /// Returns a JSON string.
        /// </summary>
        /// <param name="methodId"></param>
        /// <param name="inputParametersAsStrings"></param>
        /// <returns></returns>
        public static string TryDoRevit(string methodId, params object[] inputParametersAsStrings)
        {
            return client.TryDoRevit<dynamic>(methodId, out _, inputParametersAsStrings);
        }

        public static T TryDoRevit<T>(string methodId, out string jsonResponse, params object[] inputParametersAsStrings)
        {
            jsonResponse = client.TryDoRevit(methodId, out T responseOutput, inputParametersAsStrings);

            return responseOutput;
        }

        public static void StartSynapseClient()
        {
            client = SynapseClient.StartSynapseClient();
        }

        public static void ShutdownSynapseClient()
        {
            client.Shutdown();
        }
    }
}
