using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Synapse.Revit;

namespace SynapseAddinLoader.UI.SynapseInventory
{
    public class SynapseViewModel : ViewModelBase
    {
        private Core.Synapse synapse;

        public string SourceFilePath => synapse.SourceFilePath;
        public string Name => synapse.Name;
        public string Id => synapse.Id;

        public SynapseViewModel(Core.Synapse synapse)
        {
            this.synapse = synapse;
        }
      
        public Type GetRevitSynapseType()
        {
            return synapse.Type;
        }

        public IEnumerable<SynapseMethodViewModel> GetRevitMethodsFromSynapse()
        {
            MethodInfo[] methods = synapse.Type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
            foreach (MethodInfo methodInfo in methods)
            {
                if (methodInfo.GetCustomAttribute<SynapseRevitMethodAttribute>() is not SynapseRevitMethodAttribute revitCommandAttribute)
                {
                    continue;
                }

                IList<SynapseMethodInputParameterViewModel> inputParameterViewModels = new List<SynapseMethodInputParameterViewModel>();
                foreach (var methodParameter in GetMethodParameters(methodInfo))
                {
                    SynapseMethodInputParameterViewModel inputParameterViewModel = new SynapseMethodInputParameterViewModel(methodParameter.ParameterName, methodParameter.ParameterType.Name)
                    {
                        Value = methodParameter.DefaultValue?.ToString() ?? ""
                    };

                    inputParameterViewModels.Add(inputParameterViewModel);
                }

                yield return new SynapseMethodViewModel(revitCommandAttribute.MethodId, methodInfo.Name, inputParameterViewModels, methodInfo.ReturnType);
            }
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
