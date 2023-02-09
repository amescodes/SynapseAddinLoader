using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Autodesk.Revit.DB;
using SynapseAddinLoader.Core.Domain;
using SynapseAddinLoader.Core;

using Synapse.Revit;
using Synapse.Revit.Service;
using Synapse.Shared;

namespace SynapseAddinLoader.Revit
{
    public class LoadSynapses
    {
        private string assemblyPath;

        public LoadSynapses(string assemblyPath)
        {
            this.assemblyPath = assemblyPath;
        }

        /// <exception cref="SynapseAddinLoaderException"></exception>
        public IList<Core.Domain.Synapse> GetSynapsesFromFile()
        {
            ResolveAssembly resolveAssembly = new ResolveAssembly(Path.GetDirectoryName(assemblyPath));
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
            
            return MakeSynapsesFromFoundTypes(foundSynapseTypes);
        }

        private IList<Core.Domain.Synapse> MakeSynapsesFromFoundTypes(IList<Type> foundSynapseTypes)
        {
            IList<Core.Domain.Synapse> synapses = new List<Core.Domain.Synapse>();
            foreach (Type synapseType in foundSynapseTypes)
            {
                IList<SynapseMethod> synapseMethods = MakeSynapseMethods(synapseType);

                synapses.Add(new Core.Domain.Synapse(assemblyPath, synapseType.Name,synapseType,  synapseMethods));
            }

            return synapses;
        }

        private IList<SynapseMethod> MakeSynapseMethods(Type synapseType)
        {
            MethodInfo[] methods =
                synapseType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
            IList<SynapseMethod> synapseMethods = new List<SynapseMethod>();
            foreach (MethodInfo methodInfo in methods)
            {
                if (methodInfo.GetCustomAttribute<SynapseRevitMethodAttribute>() is not SynapseRevitMethodAttribute
                    revitCommandAttribute)
                {
                    continue;
                }

                IList<SynapseMethodParameter> methodParameters = MakeSynapseMethodParameters(methodInfo);

                SynapseMethod synapseMethod = new SynapseMethod(methodInfo.Name, revitCommandAttribute.MethodId,
                    methodInfo.ReturnType, methodParameters);
                synapseMethods.Add(synapseMethod);
            }

            return synapseMethods;
        }

        private IList<SynapseMethodParameter> MakeSynapseMethodParameters(MethodInfo methodInfo)
        {
            IList<SynapseMethodParameter> methodParameters = new List<SynapseMethodParameter>();
            foreach ((string parameterName, Type parameterType, object defaultValue) in GetMethodParameters(methodInfo))
            {
                SynapseMethodParameter inputParameterViewModel = new SynapseMethodParameter(parameterName, parameterType)
                {
                    Value = defaultValue
                };

                methodParameters.Add(inputParameterViewModel);
            }

            return methodParameters;
        }

        private IEnumerable<(string ParameterName, Type ParameterType, object DefaultValue)> GetMethodParameters(MethodInfo methodInfo)
        {
            foreach (ParameterInfo parameterInfo in methodInfo.GetParameters())
            {
                object defaultValue = null;
                if (parameterInfo.HasDefaultValue)
                {
                    defaultValue = parameterInfo.DefaultValue;
                }

                yield return (parameterInfo.Name, parameterInfo.ParameterType, defaultValue);
            }
        }
    }
}
