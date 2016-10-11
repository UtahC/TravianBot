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

        public virtual async Task<StateBase> Start(CancellationToken cancellationToken)
        {
            await Task.Delay(1);
            cancellationToken.ThrowIfCancellationRequested();
            retryCounter++;

            return null;
        }
    }
}
