using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynapseAddinLoader.Core
{
    [Serializable]
    public class SynapseAddinLoaderException : Exception
    {
        public SynapseAddinLoaderException() {}

        public SynapseAddinLoaderException(string message) : base(message) {}

        public SynapseAddinLoaderException(string message, Exception innerException) : base(message,innerException) {}
    }
}
