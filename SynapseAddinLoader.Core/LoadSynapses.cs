using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

using static SynapseAddinLoader.Core.Common;

using Synapse.Revit;
using Autodesk.Revit.UI;
using static Autodesk.Internal.Windows.SwfMediaPlayer;
using System.Threading.Tasks;

namespace SynapseAddinLoader.Core
{
    public class LoadSynapses
    {
        private string assemblyPath;

        public LoadSynapses(string assemblyPath)
        {
            this.assemblyPath = assemblyPath;
        }

        /// <exception cref="SynapseAddinLoaderException"></exception>
        public IList<Synapse> GetSynapsesFromFile(UIApplication uiapp)
        {
            string assemblyDirectory = Path.GetDirectoryName(assemblyPath);
            ResolveAssembly resolveAssembly = new ResolveAssembly(assemblyDirectory);
            resolveAssembly.AddResolveAssemblyHandler();

            Assembly assemblyFromPath = resolveAssembly.LoadFromPath(assemblyPath);
            IList<Type> foundSynapseTypes = new List<Type>();
            foreach (Type t in assemblyFromPath.ExportedTypes)
            {
                Type[] interfaces = t.GetInterfaces();
                if (interfaces.Select(i => i.Name.ToString()).Contains(nameof(IRevitSynapse)))
                {
                    foundSynapseTypes.Add(t);
                }
            }

            if (foundSynapseTypes.Count == 0)
            {
                throw new SynapseAddinLoaderException("No IRevitSynapse types found in assembly.");
            }

            IList<Synapse> synapses = MakeSynapsesFromFoundTypes(uiapp, foundSynapseTypes);

            resolveAssembly.RemoveResolveAssemblyHandler();

            return synapses;
        }

        private IList<Synapse> MakeSynapsesFromFoundTypes(UIApplication uiapp, IList<Type> foundSynapseTypes)
        {
            IList<Synapse> synapses = new List<Synapse>();
            foreach (Type synapseType in foundSynapseTypes)
            {
                IRevitSynapse synapseToRegister = ConstructTemporaryRevitSynapse(uiapp, synapseType);
                string synapseId = synapseType.GetRuntimeProperty(nameof(IRevitSynapse.Id)).GetValue(synapseToRegister) as string;
                synapses.Add(new Synapse(assemblyPath, synapseId, synapseType));
            }

            return synapses;
        }
    }
}
