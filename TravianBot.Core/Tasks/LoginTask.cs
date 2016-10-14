using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TravianBot.Core;

namespace TravianBot.Core.Tasks
{
    class LoginTask : TaskBase
    {
        public static async Task<bool> Execute(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            client.ExecuteJavascript(ScriptGenerator.GetLoginScript
                (client.Setting.Account, client.Setting.Password, client.Setting.IsLowResolution));
            
            if (!UtilityTask.IsLogon())
                return false;

            return true;
        }
    }
}
