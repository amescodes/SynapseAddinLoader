using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using SynapseAddinLoader.Core;
using SynapseAddinLoader.Revit;

using Synapse.Revit;
using Synapse.Revit.Service;

namespace SynapseAddinLoader
{
    public class AddinLoaderRevitSynapse : IRevitSynapse
    {
        public string Id => "c4dba387-0607-47bd-a828-82df40b0b4ec";

        public string ProcessPath => Path.Combine(Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath)!, "Client\\SynapseAddinLoader.Client.exe");

        [SynapseRevitMethod(RevitCommandRegister.LoadAssemblyId)]
        public IList<Core.Domain.Synapse> LoadSynapseAssembly(string assemblyPath)
        {
            if (!File.Exists(assemblyPath))
            {
                return null;
            }

            LoadSynapses loadSynapses = new LoadSynapses(assemblyPath);
            IList<Core.Domain.Synapse> synapsesFromFile = loadSynapses.GetSynapsesFromFile();

            foreach (Core.Domain.Synapse synapse in synapsesFromFile)
            {
                if (SynapseRevitService.GetSynapseById(synapse.Id) is SynapseProcess previousLoadedSynapse)
                {
                    SynapseRevitService.DisconnectSynapse(previousLoadedSynapse.Synapse);
                }

                //! register in synapse
                IRevitSynapse temporaryRevitSynapse = Common.ConstructTemporaryRevitSynapse(synapse.Type);
                SynapseRevitService.ConnectSynapse(temporaryRevitSynapse);
            }

            return synapsesFromFile;
        }
    }
}
