using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.UI;
using Synapse.Revit.Service;

namespace SynapseAddinLoader.Core
{
    public class Common
    {
        public static IRevitSynapse ConstructTemporaryRevitSynapse(Type synapseType)
        {
            IRevitSynapse temporarySynapseToParse = null;
            foreach (ConstructorInfo constructorInfo in synapseType.GetTypeInfo().DeclaredConstructors)
            {
                ParameterInfo[] constructorParameters = constructorInfo.GetParameters();
                if (constructorParameters.Length != 0)
                {
                    continue;
                }

                temporarySynapseToParse = constructorInfo.Invoke(new object[] { }) as IRevitSynapse;
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
