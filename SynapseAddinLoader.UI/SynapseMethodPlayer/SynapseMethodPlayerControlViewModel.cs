using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Synapse;
using SynapseAddinLoader.Core;
using SynapseAddinLoader.UI.SynapseInventory;

namespace SynapseAddinLoader.UI.SynapseMethodPlayer
{
    public class SynapseMethodPlayerControlViewModel : ViewModelBase
    {
        private SynapseClient synapseClient;

        private const string defaultSelectionMessage = "No Synapse selected";
        public string SelectedSynapseName { get; set; } = defaultSelectionMessage;

        public ObservableCollection<SynapseMethodViewModel> SynapseMethods { get; set; }

        public ICommand RunRevitMethodCommand => new RelayCommand(RunRevitMethod);

        public SynapseMethodPlayerControlViewModel(SynapseClient synapseClient)
        {
            this.synapseClient = synapseClient;
        }

        private void RunRevitMethod(object obj)
        {
            if (obj is not SynapseMethodViewModel synapseMethodToRun)
            {
                return;
            }

            RunSynapseMethodInRevitRequest<dynamic> request = new RunSynapseMethodInRevitRequest<dynamic>(synapseClient, synapseMethodToRun.MethodId);

            //todo verify parameters
            IEnumerable<string> inputParametersAsStrings = synapseMethodToRun.InputParameters.Select(p=>p.Value);

            request.RunRevitMethod(inputParametersAsStrings);
        }
    }
}
