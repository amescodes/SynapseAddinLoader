using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Microsoft.Win32;
using FileDialog = Microsoft.Win32.FileDialog;

using SynapseAddinLoader.Core;
using SynapseAddinLoader.Core.Domain;

using Synapse.Shared;

using Newtonsoft.Json;

namespace SynapseAddinLoader.Client.SynapseInventory
{
    public class SynapseInventoryControlViewModel : ViewModelBase
    {
        public ObservableCollection<SynapseViewModel> Synapses { get; }

        public ICommand LoadSynapsesFromFileCommand => new RelayCommand(_ => LoadAssemblyFiles());
        public ICommand RemoveSynapseCommand => new RelayCommand(RemoveSynapse);
        
        public SynapseInventoryControlViewModel(IEnumerable<SynapseViewModel> synapses)
        {
            Synapses = new ObservableCollection<SynapseViewModel>(synapses);
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
                    AddSynapse(synapseViewModel);
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
                IEnumerable<SynapseMethodViewModel> revitMethodsFromSynapse = GetRevitMethodsFromSynapse(synapse);
                SynapseViewModel synapseViewModel = new SynapseViewModel(synapse)
                {
                    Methods = new ObservableCollection<SynapseMethodViewModel>(revitMethodsFromSynapse)
                };
                synapseViewModels.Add(synapseViewModel);
            }

            return synapseViewModels;
        }

        private IEnumerable<SynapseMethodViewModel> GetRevitMethodsFromSynapse(Core.Domain.Synapse synapse)
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
        
        private void AddSynapse(SynapseViewModel synapseViewModel)
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

            //todo save user settings
        }

        private void RemoveSynapse(object obj)
        {
            if (obj is not SynapseViewModel synapseToRemove)
            {
                return;
            }
            
            Synapses.Remove(synapseToRemove);

            //todo save user settings
        }
    }
}
