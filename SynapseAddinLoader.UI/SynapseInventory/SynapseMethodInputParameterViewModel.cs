using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SynapseAddinLoader.UI.SynapseInventory
{
    public class SynapseMethodInputParameterViewModel : ViewModelBase
    {
        public string Name { get; }
        public string TypeName { get; }
        public string Value { get; set; }

        public SynapseMethodInputParameterViewModel(string name, string typeName)
        {
            Name = name;
            TypeName = typeName;
        }
    }
}
