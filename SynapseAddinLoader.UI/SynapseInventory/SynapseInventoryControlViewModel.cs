using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Win32;
using FileDialog = Microsoft.Win32.FileDialog;

using SynapseAddinLoader.Core;
using SynapseAddinLoader.UI.SynapseMethodPlayer;

using Autodesk.Revit.UI;

using Synapse;
using Synapse.Revit;

namespace SynapseAddinLoader.UI.SynapseInventory
{
    public class SynapseInventoryControlViewModel : ViewModelBase
    {
        private string selectedSynapseId = "";

        public SynapseMethodPlayerControlViewModel SynapseMethodPlayer { get; }
        public ObservableCollection<SynapseViewModel> Synapses { get; }

        public ICommand LoadSynapsesFromFileCommand => new RelayCommand(_ => LoadAssemblyFiles());
        public ICommand RemoveSelectedSynapseCommand => new RelayCommand(_=>RemoveSynapse(selectedSynapseId));
        
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

            selectedSynapseId = synapseSelection.Id;
            SynapseMethodPlayer.SelectedSynapseName = synapseSelection.Name;
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

            UIApplication uiapp = App.UiApplication;
            IList<SynapseViewModel> synapsesFromAssembly = ParseSynapsesFromAssembly(uiapp, assemblyPath);
            foreach (SynapseViewModel synapseViewModel in synapsesFromAssembly)
            {
                int insertionIndex = Synapses.Count; // insert at end by default
                try
                {
                    var synapseTypeToRegister = synapseViewModel.GetRevitSynapseType();
                    IRevitSynapse temporaryRevitSynapse =
                        Common.ConstructTemporaryRevitSynapse(uiapp, synapseTypeToRegister);
                    if (Synapses.FirstOrDefault(s => s.Id.Equals(synapseViewModel.Id)) is SynapseViewModel
                        previouslyAddedSynapse)
                    {
                        SynapseRevitService.DeregisterSynapse(temporaryRevitSynapse);
                        // synapse (or one with the same id) has been added before
                        // assume its the same synapse being updated
                        insertionIndex = Synapses.IndexOf(previouslyAddedSynapse);
                        Synapses.Remove(previouslyAddedSynapse);
                    }

                    //! register in synapse
                    SynapseRevitService.RegisterSynapse(temporaryRevitSynapse);

                    Synapses.Insert(insertionIndex, synapseViewModel);
                }
                catch (Exception ex)
                {
                    //todo log error
                }
            }
        }

        private IList<SynapseViewModel> ParseSynapsesFromAssembly(UIApplication uiapp, string assemblyPath)
        {
            LoadSynapses loadSynapses = new LoadSynapses(assemblyPath);
            IList<Core.Synapse> synapsesFromFile = loadSynapses.GetSynapsesFromFile(uiapp);

            IList<SynapseViewModel> synapseViewModels = new List<SynapseViewModel>();
            foreach (Core.Synapse synapse in synapsesFromFile)
            {
                synapseViewModels.Add(new SynapseViewModel(synapse));
            }

            return synapseViewModels;
        }
        
        private void RemoveSynapse(object obj)
        {
            //if (obj is not SynapseViewModel synapseToRemove)
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
