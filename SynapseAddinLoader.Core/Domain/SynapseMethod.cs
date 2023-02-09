using System;
using System.Collections.Generic;

namespace SynapseAddinLoader.Core.Domain;

public class SynapseMethod
{
    public string Name { get; }
    public string Id { get; }

    public Type ReturnType { get; }
    public IList<SynapseMethodParameter> Parameters { get; }

    public SynapseMethod(string name, string id, Type returnType, IList<SynapseMethodParameter> parameters)
    {
        Name = name;
        ReturnType = returnType;
        Parameters = parameters;
        Id = id;
    }
}