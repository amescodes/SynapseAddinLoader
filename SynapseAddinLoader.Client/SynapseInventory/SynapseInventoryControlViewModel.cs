using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Microsoft.Win32;
using FileDialog = Microsoft.Win32.FileDialog;

using SynapseAddinLoader.Client.SynapseMethodPlayer;
using SynapseAddinLoader.Core;

using Synapse.Shared;

using Newtonsoft.Json;

namespace SynapseAddinLoader.Client.SynapseInventory
{
    public class SynapseInventoryControlViewModel : ViewModelBase
    {
        private string selectedSynapseId = "";

        public SynapseMethodPlayerControlViewModel SynapseMethodPlayer { get; }
        public ObservableCollection<SynapseViewModel> Synapses { get; }

        public ICommand LoadSynapsesFromFileCommand => new RelayCommand(_ => LoadAssemblyFiles());
        public ICommand RemoveSelectedSynapseCommand => new RelayCommand(_ => RemoveSynapse(selectedSynapseId));

        public ICommand OnSynapseSelectionChangedCommand => new RelayCommand(UpdateSelectedSynapse);

        public SynapseInventoryControlViewModel(IEnumerable<SynapseViewModel> synapses)
        {
            Synapses = new ObservableCollection<SynapseViewModel>(synapses);
            SynapseMethodPlayer = new SynapseMethodPlayerControlViewModel();
        }

        private void UpdateSelectedSynapse(object obj)
        {
            if (obj is not SynapseViewModel synapseSelection ||
                synapseSelection.Id.Equals(selectedSynapseId))
            {
                return;
            }

            UpdateCurrentSelectionInPlayer(synapseSelection);
        }

        private void UpdateCurrentSelectionInPlayer(SynapseViewModel synapseSelection)
        {
            selectedSynapseId = synapseSelection.Id;
            SynapseMethodPlayer.SelectedSynapseName = synapseSelection.Name;
            SynapseMethodPlayer.SelectedSynapseSourceFilePath = synapseSelection.SourceFilePath;
            SynapseMethodPlayer.SynapseMethods = new ObservableCollection<SynapseMethodViewModel>(synapseSelection.GetRevitMethodsFromSynapse());
        }

        private void LoadAssemblyFiles()
        {
            FileDialog fileDialog = new OpenFileDialog()
            {
                Filter = "Assembly files (*.dll)|*.dll|All files (*.*)|*.*",
                Multiselect = true,
                CheckFileExists = true,
            };
            if (fileDialog.ShowDialog() == false)
            {
                return;
            }

            string[] assemblyPaths = fileDialog.FileNames;

            foreach (string assemblyPath in assemblyPaths)
            {
                LoadAssemblyFile(assemblyPath);
            }

            //todo save user settings
        }

        private void LoadAssemblyFile(string assemblyPath)
        {
            if (!Path.HasExtension(assemblyPath) ||
                !Path.GetExtension(assemblyPath).EndsWith("dll"))
            {
                return;
            }
            
            try
            {
                IList<SynapseViewModel> synapsesFromAssembly = ParseSynapsesFromAssembly(assemblyPath);
                foreach (SynapseViewModel synapseViewModel in synapsesFromAssembly)
                {
                    int insertionIndex = Synapses.Count; // insert at end by default
                    if (Synapses.FirstOrDefault(s => s.Id.Equals(synapseViewModel.Id)) is SynapseViewModel
                        previouslyAddedSynapse)
                    {
                        // synapse (or one with the same id) has been added before
                        // assume its the same synapse being updated
                        insertionIndex = Synapses.IndexOf(previouslyAddedSynapse);
                        Synapses.Remove(previouslyAddedSynapse);
                    }

                    Synapses.Insert(insertionIndex, synapseViewModel);
                }
            }
            catch (Exception ex)
            {
                //todo log error

            }
        }

        private IList<SynapseViewModel> ParseSynapsesFromAssembly(string assemblyPath)
        {
            if (!File.Exists(assemblyPath))
            {
                return new List<SynapseViewModel>();
            }

            Core.Domain.Synapse[] synapsesFromFile;
            string jsonResponse = "";
            try
            {
                synapsesFromFile = App.TryDoRevit<Core.Domain.Synapse[]>(RevitCommandRegister.LoadAssemblyId, out jsonResponse, assemblyPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                
                synapsesFromFile = JsonConvert.DeserializeObject<Core.Domain.Synapse[]>(jsonResponse, SynapseJsonSerializerSettings.Get()) ?? new Core.Domain.Synapse[]{};
            }

            IList<SynapseViewModel> synapseViewModels = new List<SynapseViewModel>();
            foreach (Core.Domain.Synapse synapse in synapsesFromFile)
            {
                synapseViewModels.Add(new SynapseViewModel(synapse));
            }

            return synapseViewModels;
        }

        private void RemoveSynapse(object obj)
        {
            if (obj is not string synapseIdToRemove)
            {
                return;
            }

            SynapseViewModel foundSynapse = Synapses.FirstOrDefault(s => s.Id.Equals(synapseIdToRemove));
            if (foundSynapse == null)
            {
                return;
            }

            Synapses.Remove(foundSynapse);

            //todo save user settings
        }
    }
}
