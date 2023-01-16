using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using Newtonsoft.Json;
using Revit.Async.ExternalEvents;
using Synapse;

namespace SynapseAddinLoader.Core
{
    public class SynapseCallMethodExternalEventHandler : SyncGenericExternalEventHandler<object[],string>
    {
        public override string GetName()
        {
            return nameof(SynapseCallMethodExternalEventHandler);
        }

        public override object Clone()
        {
            throw new NotImplementedException();
        }

        protected override string Handle(UIApplication app, object[] parameter)
        {
            throw new NotImplementedException();

        }
    }
}
