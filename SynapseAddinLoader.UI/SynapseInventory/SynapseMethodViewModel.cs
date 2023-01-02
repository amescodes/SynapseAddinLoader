using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

using Synapse.Revit;

namespace SynapseAddinLoader.UI.SynapseInventory
{
    public class SynapseMethodViewModel
    {
        public string MethodId { get; }
        public string MethodName { get; }

        public ObservableCollection<SynapseMethodInputParameterViewModel> InputParameters { get; }
        public Type ReturnType { get; }
        
        public SynapseMethodViewModel(string methodId, string methodName, IEnumerable<SynapseMethodInputParameterViewModel> inputs, Type returnType)
        {
            MethodId = methodId;
            MethodName = methodName;
            InputParameters = new ObservableCollection<SynapseMethodInputParameterViewModel>(inputs);
            ReturnType = returnType;
        }
    }
}