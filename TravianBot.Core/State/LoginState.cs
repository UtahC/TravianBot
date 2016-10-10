using Awesomium.Core;
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
        bool isLoginOnly = false;

        public LoginState()
        {

        }

        public LoginState(bool isLoginOnly)
        {
            this.isLoginOnly = isLoginOnly;
        }

        public override StateBase Start(CancellationToken cancellationToken)
        {
            base.Start(cancellationToken);

            client.GoUrl(client.Setting.Server.ToUri().GetSuburbsUri());
            if (client.Url == client.Setting.Server || !CheckTask.IsLogon())
            {
                client.Logger.Write("Now trying to login.");
                LoginTask.Execute(cancellationToken);
            }
                

            if (isLoginOnly)
                return null;

            return new LoginState();
        }
    }
}
