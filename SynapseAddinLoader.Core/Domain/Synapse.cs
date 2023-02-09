using System;
using System.Collections.Generic;

using SynapseAddinLoader.Core.Json;

using Newtonsoft.Json;

namespace SynapseAddinLoader.Core.Domain
{
    public class Synapse
    {
        public string SourceFilePath { get; set; }
        public string Name { get; set; }
        public string Id { get; set; }
        public IList<SynapseMethod> Methods { get; set; }

        [JsonConverter(typeof(SystemTypeConverter))] 
        public Type Type { get; set; }
        
        [JsonConstructor]
        private Synapse() { }

        public Synapse(string sourceFilePath, string id, Type type, IList<SynapseMethod> methods)
        {
            SourceFilePath = sourceFilePath;
            Type = type;
            Methods = methods;
            Name = type?.FullName;
            Id = id;
        }
    }
}
