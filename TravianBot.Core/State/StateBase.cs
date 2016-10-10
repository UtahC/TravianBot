using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TravianBot.Core.State
{
    public abstract class StateBase
    {
        protected Client client = Client.Default;
        protected int retryCounter = 0;

        public virtual StateBase Start(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            retryCounter++;

            return null;
        }
    }
}
