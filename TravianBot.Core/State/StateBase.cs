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
        protected int retryCount = 0;
        protected int retryCountLimit = 3;
        protected string retryLogMessage { get; set; }
        protected bool isTaskWorked = true;

        public virtual async Task<StateBase> Start(CancellationToken cancellationToken)
        {
            await Task.Delay(1);
            cancellationToken.ThrowIfCancellationRequested();

            if (retryCount > 0 && retryCount < retryCountLimit)
                client.Logger.Write($"{retryLogMessage}, try again. ({retryCount}/{retryCountLimit})");
            else
                client.Logger.Write($"{retryLogMessage} ({retryCount}/{retryCountLimit})");


            retryCount++;

            return null;
        }
    }
}
