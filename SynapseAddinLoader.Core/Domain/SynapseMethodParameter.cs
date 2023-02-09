using System;

namespace SynapseAddinLoader.Core.Domain;

public class SynapseMethodParameter
{
    public string Name { get; }
    public Type Type { get; }
    public object Value { get; set; }

    public SynapseMethodParameter(string name, Type type)
    {
        Name = name;
        Type = type;
    }
}