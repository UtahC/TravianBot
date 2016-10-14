using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravianBot.Core.Enums;
using TravianBot.Core.Extensions;

namespace TravianBot.Core
{
    class UrlGenerator
    {
        private const string UrlSuburbs = "dorf1.php";
        private const string UrlCity = "dorf2.php";
        private const string UrlBuilding = "build.php";
        private const string UrlReport = "berichte.php";
        private const string UrlMessage = "nachrichten.php";

        public static string ServerUrl { private get; set; }

        public static string GetCityUri()
        {
            return new Uri(ServerUrl).Combine(UrlCity).AbsoluteUri;
        }

        public static string GetCityUri(int villageId)
        {
            var keyValue = new KeyValuePair<string, string>("newdid", villageId.ToString());
            return new Uri(ServerUrl).Combine(UrlCity).Combine(keyValue).AbsoluteUri;
        }

        public static string GetSuburbsUri()
        {
            return new Uri(ServerUrl).Combine(UrlSuburbs).AbsoluteUri;
        }

        public static string GetSuburbsUri(int villageId)
        {
            var keyValue = new KeyValuePair<string, string>("newdid", villageId.ToString());
            return new Uri(ServerUrl).Combine(UrlSuburbs).Combine(keyValue).AbsoluteUri;
        }

        public static string GetBuildingUri(int buildingId)
        {
            var keyValue = new KeyValuePair<string, string>("id", buildingId.ToString());
            return new Uri(ServerUrl).Combine(UrlBuilding).Combine(keyValue).AbsoluteUri;
        }

        public static string GetBuildingUri(int villageId, int buildingId)
        {
            var keyValueVillageId = new KeyValuePair<string, string>("newdid", villageId.ToString());
            var keyValueBuildingId = new KeyValuePair<string, string>("id", buildingId.ToString());
            return new Uri(ServerUrl).Combine(UrlBuilding).Combine(keyValueVillageId, keyValueBuildingId).AbsoluteUri;
        }

        public static string GetExecuteBuildUri(bool isZeroLevel, Buildings type, int buildingId, string buildCode)
        {
            var keyValueBuildCode = new KeyValuePair<string, string>("c", buildCode);

            if (isZeroLevel && (int)type > 4 && buildingId > 18)
            {
                var keyValueBuildingType = new KeyValuePair<string, string>("a", ((int)type).ToString());
                var keyValueBuildingId = new KeyValuePair<string, string>("id", buildingId.ToString());

                return new Uri(ServerUrl).Combine(UrlCity)
                    .Combine(keyValueBuildingType, keyValueBuildingId, keyValueBuildCode).AbsoluteUri;
            }
            else
            {
                var keyValueBuildingType = new KeyValuePair<string, string>("a", buildingId.ToString());

                if ((int)type <= 4 || buildingId <= 18)
                    return new Uri(ServerUrl).Combine(UrlSuburbs)
                        .Combine(keyValueBuildingType, keyValueBuildCode).AbsoluteUri;
                else
                    return new Uri(ServerUrl).Combine(UrlCity)
                        .Combine(keyValueBuildingType, keyValueBuildCode).AbsoluteUri;
            }
        }
    }
}
