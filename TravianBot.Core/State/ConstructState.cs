using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TravianBot.Core.Enums;
using TravianBot.Core.Extensions;
using TravianBot.Core.Models;
using TravianBot.Core.Tasks;

namespace TravianBot.Core.State
{
    class ConstructState : StateBase
    {
        public override async Task<StateBase> Start(CancellationToken cancellationToken)
        {
            await base.Start(cancellationToken);

            if (retryCount >= retryCountLimit)
                client.Logger.Write("Cannot construct or upgrade buildings.");

            #region test data
            var village = client.Villages.Where(v => v.VillageId == 76307).FirstOrDefault();
            village.ConstructionTasks = new List<ConstructTaskModel>()
            {
                //new ConstructTaskModel()
                //{
                //    BuildingId = 5,
                //    IsConstrution = false,
                //    BuildingType = Buildings.ClayPit,
                //    LevelAfterWork = 1
                //},
                //new ConstructTaskModel()
                //{
                //    BuildingId = 38,
                //    IsConstrution = true,
                //    BuildingType = Buildings.Granary,
                //    LevelAfterWork = 0
                //}
            };
            #endregion
            await ConstructTask.Build(village, cancellationToken);

            return null;
        }
    }
}
