using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;

using SynapseAddinLoader.Client.SynapseInventory;
using SynapseAddinLoader.Core;

namespace SynapseAddinLoader.Client.SynapseMethodPlayer
{
    public class SynapseMethodPlayerControlViewModel : ViewModelBase
    {
        private const string defaultSelectionMessage = "No Synapse selected";
        public string SelectedSynapseName { get; set; } = defaultSelectionMessage;
        public string SelectedSynapseSourceFilePath { get; set; } = "";

        public ObservableCollection<SynapseMethodViewModel> SynapseMethods { get; set; } =
            new ObservableCollection<SynapseMethodViewModel>();

        public ICommand RunRevitMethodCommand => new RelayCommand(RunRevitMethod);
        
        private void RunRevitMethod(object obj)
        {
            if (obj is not SynapseMethodViewModel synapseMethodToRun)
            {
                return;
            }

            try
            {
                //todo verify parameters
                IEnumerable<string> inputParametersAsStrings = synapseMethodToRun.InputParameters.Select(p => p.Value);
                if (!inputParametersAsStrings.Any())
                {
                    inputParametersAsStrings = null;
                }

                //Autodesk.Windows.ComponentManager.Ribbon.Dispatcher.Invoke(() =>
                //{
                //    string responseFromRevit = App.TryDoRevit(synapseMethodToRun.MethodId, inputParametersAsStrings);
                //}
                string responseFromRevit = App.TryDoRevit(synapseMethodToRun.MethodId, inputParametersAsStrings);

            }
            catch (Exception ex)
            {

            }
            finally
            {

            }
        }
    }
}
