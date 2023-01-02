using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.UI;

using Synapse.Revit;

namespace SynapseAddinLoader.Core
{
    public class Common
    {
        public static UIApplication UiApplication { get; set; }

        public static IRevitSynapse ConstructTemporaryRevitSynapse(UIApplication uiapp, Type synapseType)
        {
            IRevitSynapse temporarySynapseToParse = null;
            foreach (ConstructorInfo constructorInfo in synapseType.GetTypeInfo().DeclaredConstructors)
            {
                ParameterInfo[] constructorParameters = constructorInfo.GetParameters();
                if (constructorParameters.Length != 1 ||
                    constructorParameters.FirstOrDefault()?.ParameterType != typeof(UIApplication))
                {
                    continue;
                }

                temporarySynapseToParse = constructorInfo.Invoke(new object[] { uiapp }) as IRevitSynapse;
                if (temporarySynapseToParse != null)
                {
                    break;
                }
            }

            if (temporarySynapseToParse == null)
            {
                throw new ArgumentException($"Revit synapse can't be constructed! Implement a constructor with only an UIApplication parameter for type {synapseType.FullName}");
            }

            return temporarySynapseToParse;
        }
    }
}
