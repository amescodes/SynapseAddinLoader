using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public ObservableCollection<SynapseMethodViewModel> Methods { get; set; }

        public SynapseViewModel(Core.Domain.Synapse synapse)
        {
            this.synapse = synapse;
        }
      
        

        //private IEnumerable<(string ParameterName, Type ParameterType, object DefaultValue)> GetMethodParameters(MethodInfo methodInfo)
        //{
        //    foreach (ParameterInfo parameterInfo in methodInfo.GetParameters())
        //    {
        //        object defaultValue = null;
        //        if (parameterInfo.HasDefaultValue)
        //        {
        //            defaultValue = parameterInfo.DefaultValue;
        //        }

        //        yield return (parameterInfo.Name, parameterInfo.ParameterType, defaultValue);
        //    }
        //}
    }
}
