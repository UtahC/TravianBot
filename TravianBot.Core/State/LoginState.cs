using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TravianBot.Core.Extensions;
using TravianBot.Core.Tasks;

namespace TravianBot.Core.State
{
    class LoginState : StateBase
    {
        public bool IsLoginOnly { get; set; } = false;

        public async override Task<StateBase> Start(CancellationToken cancellationToken)
        {
            await base.Start(cancellationToken);

            if (retryCount >= retryCountLimit)
                throw new Exception("Cannot login");

            client.Url = client.Setting.Server.ToUri().GetSuburbsUri().AbsoluteUri;
            if (client.Url == client.Setting.Server || !UtilityTask.IsLogon())
            {
                client.Logger.Write("Now logging in to server.");
                await LoginTask.Execute(cancellationToken);

                if (!UtilityTask.IsLogon())
                {
                    retryLogMessage = "Login failed";
                    return this;
                }

                client.Logger.Write("Logged in successfully.");
            }
            else
            {
                client.Logger.Write("Already logged in.");
            }
            
            if (IsLoginOnly)
                return null;

            return new InitializeBotState();
        }
    }
}
