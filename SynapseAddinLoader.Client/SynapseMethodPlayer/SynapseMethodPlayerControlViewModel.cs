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

            finally
            {

            }
        }
    }
}
