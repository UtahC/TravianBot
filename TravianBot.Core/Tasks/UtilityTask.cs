using Awesomium.Core;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravianBot.Core.Enums;
using TravianBot.Core.Extensions;
using TravianBot.Core.Models;

namespace TravianBot.Core.Tasks
{
    class UtilityTask : TaskBase
    {
        public static bool IsLogon()
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(client.Html);
            if (doc.GetElementbyId("sidebarBoxVillagelist") == null)
                return false;
            return true;
        }

        public void LoadVillages()
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(client.Html);
            client.Villages = new List<Village>(GetVillages());
        }

        public void LoadBuildings(int villageId)
        {
            var suburbs = GetSuburbsBuildings(villageId);
            var city = GetCityBuildings(villageId);

            client.Villages.FirstOrDefault(v => v.Id == villageId).Buildings = suburbs.Concat(city).ToList();

            #region test
            foreach (var building in suburbs)
            {
                Debug.WriteLine($"{building.Id} {building.Type} {building.Level} {building.VillageId}");
            }
            foreach (var building in city)
            {
                Debug.WriteLine($"{building.Id} {building.Type} {building.Level} {building.VillageId}");
            }
            #endregion
        }

        public List<Building> GetSuburbsBuildings(int villageId)
        {
            client.GoUrl(client.Setting.Server.ToUri().GetSuburbsUri(villageId));

            var doc = new HtmlDocument();
            doc.LoadHtml(client.Html);
            var buildings = new List<Building>();
            var buildingNodes = doc?.GetElementbyId("village_map")?.Descendants("div")?
                .Where(e => e.HasAttributeAndContainsValue("class", "gid") &&
                e.HasAttributeAndContainsValue("class", "level"));
            foreach (var buildingNode in buildingNodes)
            {
                //<div class="level colorLayer good gid1 level0" style="left: 179px; top:78px;">
                //    < div class="labelLayer"></div>
                //</div>
                int id = buildings.Count + 1;

                Buildings type = Buildings.None;
                int level = 0;
                var classValue = buildingNode.Attributes["class"]?.Value;
                foreach (var s in classValue.Split(' '))
                {
                    if (s.Contains("gid"))
                        type = (Buildings)int.Parse(s.Substring("gid"));
                    if (s.Contains("level") && s.Length > 5)
                        level = int.Parse(s.Substring("level"));
                }
                buildings.Add(new Building() { Id = id, Level = level, VillageId = villageId, Type = type });
            }

            return buildings;
        }

        public List<Building> GetCityBuildings(int villageId)
        {
            client.GoUrl(client.Setting.Server.ToUri().GetCityUri(villageId));

            var doc = new HtmlDocument();
            doc.LoadHtml(client.Html);

            var buildingLevels = doc?.GetElementbyId("levels")?.Descendants("div")?
                .Where(e => e.HasAttributeAndContainsValue("class", "colorLayer"));
            var buildingNodes = doc?.GetElementbyId("village_map")?.Descendants("img")?
                .Where(e => e.HasAttributeAndContainsValue("class", "building") ||
                (e.HasAttributeAndContainsValue("class", "wall") && e.HasAttributeAndNotContainsValue("class", "onTop")));

            var levelPairs = GetCityBuildingLevels(buildingLevels);
            var tribe = GetTribe();

            var buildings = new List<Building>();
            foreach (var buildingNode in buildingNodes)
            {
                //< img style = "left:485px; top:119px; z-index:24" 
                //    src = "img/x.gif" class="building iso" alt="建築物工地">
                //< img style = "left:249px; top:71px; z-index:20" src = "img/x.gif" 
                //    class="building g15" alt="村莊大樓 <span class=&quot;level&quot;>等級 1</span>||
                //    升級至等級 2 需花費:<br />
                int level = 0;
                int id = buildings.Count + 19;
                var type = Buildings.None;

                if (buildingNode.HasClass("iso"))
                {
                    type = Buildings.None;
                }
                else
                {
                    if (id != 39 || (id == 39 && levelPairs.ContainsKey(39)))
                        level = levelPairs[id];
                    type = (Buildings)buildingNode?.Attributes["class"]?.Value?.GetNumber();
                }

                buildings.Add(new Building() { Id = id, Type = type, VillageId = villageId, Level = level });
            }
            if (buildings.Count == 21)
            {
                Buildings type = Buildings.None;
                switch (tribe)
                {
                    case Tribes.Romans: type = Buildings.CityWall; break;
                    case Tribes.Teutons: type = Buildings.EarthWall; break;
                    case Tribes.Gauls: type = Buildings.Palisade; break;
                }
                buildings.Add(new Building() { Id = 40, Type = type, VillageId = villageId, Level = 0 });
            }

            return buildings;
        }

        public static Tribes GetTribe()
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(client.Html);
            var node = doc?.GetElementbyId("sidebarBoxHero")?.Descendants("img")?
                .Where(e => e.HasAttributeAndContainsValue("class", "nation")).FirstOrDefault();
            return (Tribes)node?.Attributes["class"]?.Value?.GetNumber();
        }
         
        public static List<Village> GetVillages()
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(client.Html);
            var villages = new List<Village>();
            var villageNodes = doc?.GetElementbyId("sidebarBoxVillagelist")?.Descendants()?
                .Where(n => n.HasAttributeAndContainsValue("class", "innerBox content"))
                .FirstOrDefault()?.Descendants("li");

            foreach (var node in villageNodes)
            {
                var id = int.Parse(node.Descendants("a")?.FirstOrDefault()?.Attributes["href"]?.Value?.Substring("newdid=", "&"));
                var name = node.Descendants("div")?
                    .Where(n => n.HasAttributeAndContainsValue("class", "name"))
                    .FirstOrDefault()?.InnerHtml;
                var xStr = node.Descendants("span")?
                    .Where(n => n.HasAttributeAndContainsValue("class", "coordinateX"))
                    .FirstOrDefault()?.InnerHtml;
                var x = int.Parse(string.Concat(xStr.Where(c => c >= 48 && c <= 57)));
                var yStr = node.Descendants("span")?
                    .Where(n => n.HasAttributeAndContainsValue("class", "coordinateY"))
                    .FirstOrDefault()?.InnerHtml;
                var y = int.Parse(string.Concat(yStr.Where(c => c >= 48 && c <= 57)));

                var village = new Village() { Name = name, Id = id, X = x, Y = y };

                if (node.Attributes.Contains("class") && node.Attributes["class"].Value.Contains("active"))
                    village.IsActive = true;
                else
                    village.IsActive = false;

                villages.Add(village);
            }

            return villages;
        }

        private static Dictionary<int, int> GetCityBuildingLevels(IEnumerable<HtmlNode> buildingLevels)
        {
            var levelPairs = new Dictionary<int, int>();
            foreach (var levelNode in buildingLevels)
            {
                //< div id = "levels" class="t44">
                //    <div style = "left:300px; top:122px" class="colorLayer notNow aid26">
                //        <div class="labelLayer">6</div>
                //    </div>
                //    <div style = "left:346px; top:342px" class="colorLayer notNow aid35">
                //        <div class="labelLayer">5</div>
                //</div>
                int lv = int.Parse(levelNode?.FirstChild?.InnerHtml);
                int buildingId = 0;
                var classValues = levelNode.Attributes["class"]?.Value?.Split(' ');
                foreach (var s in classValues)
                {
                    if (s.Contains("aid"))
                        buildingId = s.GetNumber();
                }

                levelPairs.Add(buildingId, lv);
            }

            return levelPairs;
        }

        public static string GetBuildCode()
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(client.Html);
            var buttonNode = doc?.GetElementbyId("build")?.Descendants("div")?
                .Where(n => n.HasClass("contractLink")).FirstOrDefault()?
                .Descendants("button")?.Where(n => n.HasClassExcept("gold"))?.FirstOrDefault();

            if (buttonNode == null)
            {
                buttonNode = doc?.GetElementbyId("build")?.Descendants("button")?
                    .Where(n => n.HasClassExcept("gold")).FirstOrDefault();

                if (buttonNode == null || buttonNode.HasClass("disabled"))
                    return null;
            }
            
            var onclickText = buttonNode.Attributes["onclick"]?.Value;

            return onclickText.Substring("&amp;c=", "';");
        }
    }
}
