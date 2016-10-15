using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            village.ConstructionTasks = new ObservableCollection<ConstructTaskModel>()
            {
                new ConstructTaskModel()
                {
                    BuildingId = 3,
                    IsConstrution = false,
                    BuildingType = Buildings.Woodcutter,
                    LevelAfterWork = 1
                },
                new ConstructTaskModel()
                {
                    BuildingId = 40,
                    IsConstrution = true,
                    BuildingType = Buildings.Palisade,
                    LevelAfterWork = 1
                }
            };
            #endregion
            await ConstructTask.Build(village, cancellationToken);

            return null;
        }
    }
}
