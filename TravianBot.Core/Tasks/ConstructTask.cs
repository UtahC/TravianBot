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

namespace TravianBot.Core.Tasks
{
    class ConstructTask : TaskBase
    {
        public static void ClearUselessTasks(Village village, CancellationToken cancellationToken)
        {
            var tasksToRemove = new List<ConstructTaskModel>();
            foreach (var building in village.Buildings)
            {
                tasksToRemove.AddRange(village.ConstructionTasks
                    .Where(t => t.BuildingId == building.BuildingId && t.LevelAfterWork <= building.Level));
            }
        }
        public static async Task Build(Village village, CancellationToken cancellationToken)
        {
            ClearUselessTasks(village, cancellationToken);
            foreach(var task in village.ConstructionTasks)
            {
                await Build(task.IsConstrution, village.VillageId, task.BuildingId, task.BuildingType, cancellationToken);
            }
        }

        public static async Task Build(bool isConstruction, int villageId, int buildingId, Buildings type, CancellationToken cancellationToken)
        {

            var buildCode = await GetBuildCode(isConstruction, villageId, buildingId, cancellationToken);

            if (buildCode != null)
                await client.LoadUrl(UriGenerator.GetExecuteBuildUri(isConstruction, type, buildingId, buildCode));
        }

        public static async Task<string> GetBuildCode(bool isConstruction, int villageId, int buildingId, CancellationToken cancellationToken)
        {
            HtmlNode buttonNode = HtmlNode.CreateNode("");
            if (!isConstruction)
            {
                await client.LoadUrl(UriGenerator.GetBuildingUri(villageId, buildingId));
                var doc = client.Document;
                buttonNode = doc?.GetElementbyId("build")?.Descendants("button")?
                        .Where(n => n.HasClassExcept("gold")).FirstOrDefault();

                if (buttonNode == null || buttonNode.HasClass("disabled"))
                    return null;
            }
            else
            {
                for (int i = 1; i <= 3; i++)
                {
                    var keyValue = new KeyValuePair<string, string>("category", i.ToString());
                    await client.LoadUrl(UriGenerator.GetBuildingUri(villageId, buildingId).Combine(keyValue));
                    var doc = client.Document;
                    buttonNode = doc?.GetElementbyId("build")?.Descendants("div")?
                        .Where(n => n.HasClass("contractLink")).FirstOrDefault()?
                        .Descendants("button")?.Where(n => n.HasClassExcept("gold"))?.FirstOrDefault();

                    if (buttonNode != null)
                        break;
                }

            }

            var onclickText = buttonNode?.Attributes["onclick"]?.Value;

            return onclickText?.Substring("&amp;c=", "';");
        }
    }
}
