using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Synapse;

namespace SynapseAddinLoader.Core
{
    public class RunSynapseMethodInRevitRequest<T>
    {
        private SynapseClient synapseClient;
        private string methodIdToRun;

        public RunSynapseMethodInRevitRequest(SynapseClient synapseClient, string methodId)
        {
            this.synapseClient = synapseClient;
            methodIdToRun = methodId;
        }

        public T RunRevitMethod(params object[] inputParameters)
        {
            //! synapse revit command!
            return synapseClient.DoRevit<T>(methodIdToRun, inputParameters);
        }
    }
}
