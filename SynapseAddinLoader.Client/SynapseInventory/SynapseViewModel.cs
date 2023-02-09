using System;
using System.Collections.Generic;
using System.Reflection;
using SynapseAddinLoader.Core.Domain;

namespace SynapseAddinLoader.Client.SynapseInventory
{
    public class SynapseViewModel : ViewModelBase
    {
        private Core.Domain.Synapse synapse;

        public string SourceFilePath => synapse.SourceFilePath;
        public string Name => synapse.Name;
        public string Id => synapse.Id;

        public SynapseViewModel(Core.Domain.Synapse synapse)
        {
            this.synapse = synapse;
        }
      
        public IEnumerable<SynapseMethodViewModel> GetRevitMethodsFromSynapse()
        {
            foreach (SynapseMethod synapseMethod in synapse.Methods)
            {
                IList<SynapseMethodInputParameterViewModel> inputParameterViewModels = new List<SynapseMethodInputParameterViewModel>();
                foreach (var methodParameter in synapseMethod.Parameters)
                {
                    SynapseMethodInputParameterViewModel inputParameterViewModel = new SynapseMethodInputParameterViewModel(methodParameter.Name, methodParameter.Type.Name)
                    {
                        Value = methodParameter.Value?.ToString() ?? ""
                    };

                    inputParameterViewModels.Add(inputParameterViewModel);
                }

                yield return new SynapseMethodViewModel(synapseMethod.Id, synapseMethod.Name, inputParameterViewModels, synapseMethod.ReturnType);
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
