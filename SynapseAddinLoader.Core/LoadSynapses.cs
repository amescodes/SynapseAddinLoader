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

namespace SynapseAddinLoader.Core
{
    public class LoadSynapses
    {
        private string assemblyPath;

        public LoadSynapses(string assemblyPath)
        {
            this.assemblyPath = assemblyPath;
        }

        public IList<Synapse> GetSynapsesFromFile(UIApplication uiapp)
        {
            if (!File.Exists(assemblyPath))
            {
                throw new FileNotFoundException("Assembly not found at location.", assemblyPath);
            }

            ResolveAssembly resolveAssembly = new ResolveAssembly(Path.GetDirectoryName(assemblyPath));
            resolveAssembly.AddResolveAssemblyHandler();

            Assembly assemblyFromPath = resolveAssembly.LoadFromPath(assemblyPath);
            IList<Type> foundSynapseTypes = GetSynapseTypesFromAssembly(assemblyFromPath);

            IList<Synapse> synapses = new List<Synapse>();
            foreach (Type synapseType in foundSynapseTypes)
            {
                IRevitSynapse synapseToRegister = ConstructTemporaryRevitSynapse(uiapp, synapseType);
                string synapseId = synapseType.GetRuntimeProperty(nameof(IRevitSynapse.Id)).GetValue(synapseToRegister) as string;
                synapses.Add(new Synapse(assemblyPath, synapseId, synapseType));
            }

            resolveAssembly.RemoveResolveAssemblyHandler();

            return synapses;
        }

        private static IList<Type> GetSynapseTypesFromAssembly(Assembly assembly)
        {
            IList<Type> foundSynapses = assembly.ExportedTypes.Where(t => t.GetInterfaces().Contains(typeof(IRevitSynapse))).ToList();

            if (foundSynapses.Count == 0)
            {
                throw new Exception("No IRevitSynapse types found in assembly.");
            }

            return foundSynapses;
        }
    }
}
