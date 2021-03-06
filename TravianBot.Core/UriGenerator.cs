﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravianBot.Core.Enums;
using TravianBot.Core.Extensions;

namespace TravianBot.Core
{
    public class UriGenerator
    {
        public static readonly string UrlSuburbs = "dorf1.php";
        public static readonly string UrlCity = "dorf2.php";
        public static readonly string UrlBuilding = "build.php";
        public static readonly string UrlReport = "berichte.php";
        public static readonly string UrlMessage = "nachrichten.php";

        public static string ServerUrl { private get; set; }

        public static Uri GetCityUri()
        {
            return new Uri(ServerUrl).Combine(UrlCity);
        }

        public static Uri GetCityUri(int villageId)
        {
            var keyValue = new KeyValuePair<string, string>("newdid", villageId.ToString());
            return new Uri(ServerUrl).Combine(UrlCity).Combine(keyValue);
        }

        public static Uri GetSuburbsUri()
        {
            return new Uri(ServerUrl).Combine(UrlSuburbs);
        }

        public static Uri GetSuburbsUri(int villageId)
        {
            var keyValue = new KeyValuePair<string, string>("newdid", villageId.ToString());
            return new Uri(ServerUrl).Combine(UrlSuburbs).Combine(keyValue);
        }

        public static Uri GetBuildingUri(int buildingId)
        {
            var keyValue = new KeyValuePair<string, string>("id", buildingId.ToString());
            return new Uri(ServerUrl).Combine(UrlBuilding).Combine(keyValue);
        }

        public static Uri GetBuildingUri(int villageId, int buildingId)
        {
            var keyValueVillageId = new KeyValuePair<string, string>("newdid", villageId.ToString());
            var keyValueBuildingId = new KeyValuePair<string, string>("id", buildingId.ToString());
            return new Uri(ServerUrl).Combine(UrlBuilding).Combine(keyValueVillageId, keyValueBuildingId);
        }

        public static Uri GetExecuteBuildUri(bool isZeroLevel, Buildings type, int buildingId, string buildCode)
        {
            var keyValueBuildCode = new KeyValuePair<string, string>("c", buildCode);

            if (isZeroLevel && (int)type > 4 && buildingId > 18)
            {
                var keyValueBuildingType = new KeyValuePair<string, string>("a", ((int)type).ToString());
                var keyValueBuildingId = new KeyValuePair<string, string>("id", buildingId.ToString());

                return new Uri(ServerUrl).Combine(UrlCity)
                    .Combine(keyValueBuildingType, keyValueBuildingId, keyValueBuildCode);
            }
            else
            {
                var keyValueBuildingType = new KeyValuePair<string, string>("a", buildingId.ToString());

                if ((int)type <= 4 || buildingId <= 18)
                    return new Uri(ServerUrl).Combine(UrlSuburbs)
                        .Combine(keyValueBuildingType, keyValueBuildCode);
                else
                    return new Uri(ServerUrl).Combine(UrlCity)
                        .Combine(keyValueBuildingType, keyValueBuildCode);
            }
        }
    }
}
