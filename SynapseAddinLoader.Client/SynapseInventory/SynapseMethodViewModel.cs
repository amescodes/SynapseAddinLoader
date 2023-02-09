using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace SynapseAddinLoader.Client.SynapseInventory
{
    public class SynapseMethodViewModel : ViewModelBase
    {
        public string MethodId { get; }
        public string Name { get; }

        public ObservableCollection<SynapseMethodInputParameterViewModel> InputParameters { get; }
        public Type ReturnType { get; }

        public ICommand RunRevitMethodCommand => new RelayCommand(_ => RunRevitMethod());

        public ObservableCollection<string> LatestResponses { get; } = new ObservableCollection<string>();

        public SynapseMethodViewModel(string methodId, string name, IEnumerable<SynapseMethodInputParameterViewModel> inputs, Type returnType)
        {
            MethodId = methodId;
            Name = name;
            InputParameters = new ObservableCollection<SynapseMethodInputParameterViewModel>(inputs);
            ReturnType = returnType;
        }

        private void RunRevitMethod()
        {
            try
            {
                //todo verify parameters
                IEnumerable<string> inputParametersAsStrings = InputParameters.Select(p => p.Value);
                if (!inputParametersAsStrings.Any())
                {
                    inputParametersAsStrings = null;
                }
                
                string responseFromRevit = App.TryDoRevit(MethodId, inputParametersAsStrings);
                LatestResponses.Add(responseFromRevit);
            }
            catch (Exception ex)
            {

            }
        }
    }
}