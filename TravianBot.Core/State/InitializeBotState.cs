using System.Threading;
using System.Threading.Tasks;
using TravianBot.Core.Extensions;
using TravianBot.Core.Tasks;

namespace TravianBot.Core.State
{
    public class InitializeBotState : StateBase
    {
        //set tribe
        //set villages
        public async override Task<StateBase> Start(CancellationToken cancellationToken)
        {
            await base.Start(cancellationToken);

            if (retryCount >= retryCountLimit) ;
            //todo

            UtilityTask.LoadVillages();
            client.Setting.Tribe = UtilityTask.GetTribe();

            return null;
        }
    }
}