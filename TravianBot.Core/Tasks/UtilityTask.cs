using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TravianBot.Core.Enums;
using TravianBot.Core.Extensions;
using TravianBot.Core.Models;
using System.Windows;

namespace TravianBot.Core.Tasks
{
    public class UtilityTask : TaskBase
    {
        public static bool IsLogon()
        {
            var doc = client.Document;
            if (doc.GetElementbyId("sidebarBoxVillagelist") == null)
                return false;
            return true;
        }
        //public static void LoadAllBuildings(int villageID)
        //{
        //    var buildings = GetAllBuildings(villageID);
        //    throw new NotImplementedException();
            ////client.Villages.Where(v => v.VillageId == villageID)
            ////    .FirstOrDefault().Buildings = new ObservableCollection<Building>(buildings);
        //}
        //public static async Task<IEnumerable<Building>> GetAllBuildings(int villageId)
        //{
        //    var suburbs = await GetSuburbsBuildings(villageId);
        //    var city = await GetCityBuildings(villageId);

        //    return suburbs.Concat(city);
        //}
        public static async Task LoadSuburbsBuildings(int villageId)
        {
            var village = client.Villages.Where(v => v.VillageId == villageId).FirstOrDefault();
            if (village == null)
                return;

            await client.LoadUrl(UriGenerator.GetSuburbsUri(villageId));
            var doc = client.Document;
            if (doc == null)
                return;
            
            var newSuburbsBuildings = GetSuburbsBuildings(villageId, doc);
            var oldCityBuildings = village.Buildings.Where(v => v.BuildingId > 18);
            var buildings = newSuburbsBuildings.Concat(oldCityBuildings);
            village.Buildings = new List<Building>(buildings);
        }
        public static IEnumerable<Building> GetSuburbsBuildings(int villageId, HtmlDocument doc)
        {
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
                buildings.Add(new Building() { BuildingId = id, Level = level, VillageId = villageId, BuildingType = type });
            }

            return buildings;
        }
        public static async Task LoadCityBuildings(int villageId)
        {
            var village = client.Villages.Where(v => v.VillageId == villageId).FirstOrDefault();
            if (village == null)
                return;

            await client.LoadUrl(UriGenerator.GetCityUri(villageId));
            var doc = client.Document;
            if (doc == null)
                return;

            var newCityBuildings = GetCityBuildings(villageId, doc);
            var oldSuburbsBuildings = village.Buildings.Where(v => v.BuildingId <= 18);
            var buildings = oldSuburbsBuildings.Concat(newCityBuildings);
            village.Buildings = new List<Building>(buildings);
        }
        public static IEnumerable<Building> GetCityBuildings(int villageId, HtmlDocument doc)
        {
            var buildingLevels = doc?.GetElementbyId("levels")?.Descendants("div")?
                .Where(e => e.HasAttributeAndContainsValue("class", "colorLayer"));
            var buildingNodes = doc?.GetElementbyId("village_map")?.Descendants("img")?
                .Where(e => e.HasAttributeAndContainsValue("class", "building") ||
                (e.HasAttributeAndContainsValue("class", "wall") && e.HasAttributeAndNotContainsValue("class", "onTop")));

            var levelPairs = GetCityBuildingLevels(buildingLevels);
            var tribe = client.Setting.Tribe;

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

                buildings.Add(new Building() { BuildingId = id, BuildingType = type, VillageId = villageId, Level = level });
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
                buildings.Add(new Building() { BuildingId = 40, BuildingType = type, VillageId = villageId, Level = 0 });
            }

            return buildings;
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
        public static Tribes GetTribe()
        {
            Tribes tribe;
            var doc = client.Document;
            var tribeString = doc.GetElementbyId("sidebarBoxHero").Descendants("div")
                .Where(n => n.HasClass("playerName")).FirstOrDefault().
                Descendants("img").FirstOrDefault().Attributes["alt"].Value;
            bool success = Enum.TryParse(tribeString, true, out tribe);
            if (!success)
                tribe = Tribes.None;

            return tribe;
        }
        public static int GetUpdatedActivedVillage()
        {
            var doc = client.Document;
            return GetUpdatedActivedVillage(doc);
        }
        public static int GetUpdatedActivedVillage(HtmlDocument doc)
        {
            var id = -1;
            var villageNodes = doc?.GetElementbyId("sidebarBoxVillagelist")?.Descendants()?
                .Where(n => n.HasAttributeAndContainsValue("class", "innerBox content"))
                .FirstOrDefault()?.Descendants("li");

            if (villageNodes == null)
                return 0;

            var activedVillages = client.Villages.Where(v => v.IsActive);
            if (activedVillages != null)
            {
                foreach (var village in activedVillages)
                    village.IsActive = false;
            }
            

            foreach (var node in villageNodes)
            {
                //if (node.Attributes.Contains("class") && node.Attributes["class"].Value.Contains("active"))
                if (node.HasClass("active"))
                {
                    id = int.Parse(node.Descendants("a")?.FirstOrDefault()?
                    .Attributes["href"]?.Value?.Substring("newdid=", "&"));

                    client.Villages.Where(v => v.VillageId == id).FirstOrDefault().IsActive = true;
                    break;
                }
            }

            return id;
        }
    }

    public class UITask : TaskBase
    {
        public static int GetUpdatedActivedVillage(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            return UtilityTask.GetUpdatedActivedVillage(doc);
        }
        public static void LoadVillages(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var villages = client.Villages;
            var newVillages = GetVillages(doc);

            LoadVillages(villages, newVillages);
        }
        public static void LoadVillages(ObservableCollection<Village> villages, IEnumerable<Village> newVillages)
        {
            if (newVillages == null)
                return;

            //Remove villages which are not exist now
            var newVillageIds = newVillages.Select(v => v.VillageId);
            var villagesToBeRemoved = villages.Where(v => !newVillageIds.Contains(v.VillageId)).ToList();
            foreach (var village in villagesToBeRemoved)
                villages.Remove(village);

            //Add villages which are not exist in bot
            var villageIds = villages.Select(v => v.VillageId);
            var villagesToBeAdded = newVillages.Where(v => !villageIds.Contains(v.VillageId)).ToList();
            foreach (var village in villagesToBeAdded)
                villages.Add(village);

            //Update villages which are not equal completely
            var villagesWhereIdExists = newVillages.Where(v => villageIds.Contains(v.VillageId));
            var villagesToBeUpdated = villagesWhereIdExists
                .Where(newV => !villages.Any(oldV => oldV.Equals(newV)));
            foreach (var village in villagesToBeUpdated)
                villages.UpdateVillageById(village);
        }
        public static IEnumerable<Village> GetVillages(HtmlDocument doc)
        {
            var villages = new List<Village>();
            var villageNodes = doc?.GetElementbyId("sidebarBoxVillagelist")?.Descendants()?
                .Where(n => n.HasAttributeAndContainsValue("class", "innerBox content"))
                .FirstOrDefault()?.Descendants("li");

            if (villageNodes == null)
                return null;

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

                var village = new Village(id, name, x, y);

                if (node.Attributes.Contains("class") && node.Attributes["class"].Value.Contains("active"))
                    village.IsActive = true;
                else
                    village.IsActive = false;

                villages.Add(village);
            }

            return villages;
        }
        public static void LoadCurrentBuildings(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var activeVillage = client.Villages?.Where(v => v.IsActive == true).FirstOrDefault();
            if (activeVillage == null)
                return;

            if (client.Url.Contains(UriGenerator.UrlSuburbs))
                LoadSuburbsBuildings(doc);
            else if (client.Url.Contains(UriGenerator.UrlCity))
                LoadCityBuildings(doc);
        }

        private static void LoadSuburbsBuildings(HtmlDocument doc)
        {
            var village = client.Villages.Where(v => v.IsActive == true).FirstOrDefault();
            if (doc == null || village == null)
                return;

            var newSuburbsBuildings = UtilityTask.GetSuburbsBuildings(village.VillageId, doc);
            var oldCityBuildings = village.Buildings.Where(v => v.BuildingId > 18);
            var buildings = newSuburbsBuildings.Concat(oldCityBuildings);
            village.Buildings = new List<Building>(buildings);
        }
        private static void LoadCityBuildings(HtmlDocument doc)
        {
            var village = client.Villages.Where(v => v.IsActive == true).FirstOrDefault();
            if (doc == null || village == null)
                return;

            var newCityBuildings = UtilityTask.GetCityBuildings(village.VillageId, doc);
            var oldSuburbsBuildings = village.Buildings.Where(v => v.BuildingId <= 18);
            var buildings = oldSuburbsBuildings.Concat(newCityBuildings);
            village.Buildings = new List<Building>(buildings);
        }
    }
}
