﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using SynapseAddinLoader.Core;
using SynapseAddinLoader.UI.SynapseInventory;

using Synapse;

namespace SynapseAddinLoader.UI.SynapseMethodPlayer
{
    public class SynapseMethodPlayerControlViewModel : ViewModelBase
    {
        private const string defaultSelectionMessage = "No Synapse selected";
        public string SelectedSynapseName { get; set; } = defaultSelectionMessage;
        public string SelectedSynapseSourceFilePath { get; set; } = "";

        public ObservableCollection<SynapseMethodViewModel> SynapseMethods { get; set; }

        public ICommand RunRevitMethodCommand => new RelayCommand(x => RunRevitMethod(x));
        
        private void RunRevitMethod(object obj)
        {
            if (obj is not SynapseMethodViewModel synapseMethodToRun)
            {
                return;
            }

            ResolveAssembly resolve = new ResolveAssembly(Path.GetDirectoryName(SelectedSynapseSourceFilePath));
            try
            {
                resolve.AddResolveAssemblyHandler();

                //todo verify parameters
                IEnumerable<string> inputParametersAsStrings = synapseMethodToRun.InputParameters.Select(p => p.Value);
                if (!inputParametersAsStrings.Any())
                {
                    inputParametersAsStrings = null;
                }

                Autodesk.Windows.ComponentManager.Ribbon.Dispatcher.Invoke(() =>
                {
                    string responseFromRevit = App.TryDoRevit(synapseMethodToRun.MethodId, inputParametersAsStrings);
                });
                //string responseFromRevit =  App.TryDoRevit(synapseMethodToRun.MethodId, inputParametersAsStrings);

            }
            catch (Exception ex)
            {

            }
            finally
            {
                resolve.RemoveResolveAssemblyHandler();
            }
        }
    }
}
