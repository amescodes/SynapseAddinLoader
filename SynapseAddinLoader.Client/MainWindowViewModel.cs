using System.Collections.Generic;
using SynapseAddinLoader.Client.SynapseInventory;

namespace SynapseAddinLoader.Client
{
    public class MainWindowViewModel : ViewModelBase
    {
        public SynapseInventoryControlViewModel InventoryControl { get; }

        public MainWindowViewModel() : this(new List<SynapseViewModel>()) { }
        
        public MainWindowViewModel(IEnumerable<SynapseViewModel> synapses)
        {
            InventoryControl = new SynapseInventoryControlViewModel(synapses);
        }
    }
}
