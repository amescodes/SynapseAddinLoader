using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SynapseAddinLoader.UI.SynapseInventory;
using SynapseAddinLoader.UI.SynapseMethodPlayer;

using Synapse;

namespace SynapseAddinLoader.UI
{
    public class MainWindowViewModel : ViewModelBase
    {
        public string WindowName { get; }

        public SynapseInventoryControlViewModel InventoryControl { get; }

        public MainWindowViewModel(string windowName, SynapseClient synapseClient) : this(windowName, synapseClient, new List<SynapseViewModel>()) { }
        
        public MainWindowViewModel(string windowName, SynapseClient synapseClient, IEnumerable<SynapseViewModel> synapses)
        {
            WindowName = windowName;
            InventoryControl = new SynapseInventoryControlViewModel(synapseClient,synapses);
        }
    }
}
