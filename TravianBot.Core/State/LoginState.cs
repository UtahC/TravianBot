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

        public LoginState()
        {

        }

        public async override Task<StateBase> Start(CancellationToken cancellationToken)
        {
            await base.Start(cancellationToken);

            IsWorking = true;
            client.Url = client.Setting.Server.ToUri().GetSuburbsUri().AbsoluteUri;
            if (client.Url == client.Setting.Server || !UtilityTask.IsLogon())
            {
                client.Logger.Write("Now trying to login.");
                LoginTask.Execute(cancellationToken);
                await Task.Delay(5000);
            }

            //Login failed
            if (!UtilityTask.IsLogon())
                return this;

            //Login success
            IsWorking = false;
            if (IsLoginOnly)
                return null;
            return new InitializeBotState();
        }
    }
}
