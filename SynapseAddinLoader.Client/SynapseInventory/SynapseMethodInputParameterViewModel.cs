namespace SynapseAddinLoader.Client.SynapseInventory
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
