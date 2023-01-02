using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Synapse.Revit;

namespace SynapseAddinLoader.Core
{
    public class Synapse
    {
        public string SourceFilePath { get; }
        public Type Type { get; }
        public string Name { get; }
        public string Id { get; }

        public Synapse(string sourceFilePath, string id, Type type)
        {
            SourceFilePath = sourceFilePath;
            Type = type;
            Name = type.FullName;
            Id = id;
        }
    }
}
