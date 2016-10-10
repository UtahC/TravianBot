using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravianBot.Core.Enums;

namespace TravianBot.Core.Extensions
{
    static class UriExtension
    {
        private const string UrlSuburbs = "dorf1.php";
        private const string UrlCity = "dorf2.php";
        private const string UrlBuilding = "build.php";
        private const string UrlReport = "berichte.php";
        private const string UrlMessage = "nachrichten.php";

        public static Uri Combine(this Uri uri, params string[] subUrl)
        {
            var result = new Uri(uri.AbsoluteUri);
            for (int i = 0; i < subUrl.Length; i++)
            {
                result = new Uri(result, subUrl[i]);
            }
            return result;
        }

        public static Uri Combine(this Uri uri, params KeyValuePair<string, string>[] keyValues)
        {
            var str = string.IsNullOrEmpty(uri.Query) ? uri.AbsoluteUri + "?" : uri.AbsoluteUri;
            foreach (var keyValue in keyValues)
            {
                str += string.Format($"{keyValue.Key}={keyValue.Value}&");
            }
            return new Uri(str);
        }

        public static Uri GetCityUri(this Uri uri)
        {
            return uri.Combine(UrlCity);
        }

        public static Uri GetCityUri(this Uri uri, int villageId)
        {
            var keyValue = new KeyValuePair<string, string>("newdid", villageId.ToString());
            return uri.Combine(UrlCity).Combine(keyValue);
        }

        public static Uri GetSuburbsUri(this Uri uri)
        {
            return uri.Combine(UrlSuburbs);
        }

        public static Uri GetSuburbsUri(this Uri uri, int villageId)
        {
            var keyValue = new KeyValuePair<string, string>("newdid", villageId.ToString());
            return uri.Combine(UrlSuburbs).Combine(keyValue);
        }

        public static Uri GetBuildingUri(this Uri uri, int buildingId)
        {
            var keyValue = new KeyValuePair<string, string>("id", buildingId.ToString());
            return uri.Combine(UrlBuilding).Combine(keyValue);
        }

        public static Uri GetBuildingUri(this Uri uri, int villageId, int buildingId)
        {
            var keyValueVillageId = new KeyValuePair<string, string>("newdid", villageId.ToString());
            var keyValueBuildingId = new KeyValuePair<string, string>("id", buildingId.ToString());
            return uri.Combine(UrlBuilding).Combine(keyValueVillageId, keyValueBuildingId);
        }

        public static Uri GetExecuteBuildUri(this Uri uri, bool isZeroLevel, Buildings type, int buildingId, string buildCode)
        {
            var keyValueBuildCode = new KeyValuePair<string, string>("c", buildCode);

            if (isZeroLevel && (int)type > 4 && buildingId > 18)
            {
                var keyValueBuildingType = new KeyValuePair<string, string>("a", ((int)type).ToString());
                var keyValueBuildingId = new KeyValuePair<string, string>("id", buildingId.ToString());

                return uri.Combine(UrlCity).Combine(keyValueBuildingType, keyValueBuildingId, keyValueBuildCode);
            }
            else
            {
                var keyValueBuildingType = new KeyValuePair<string, string>("a", buildingId.ToString());

                if ((int)type <= 4 || buildingId <= 18)
                    return uri.Combine(UrlSuburbs)
                        .Combine(keyValueBuildingType, keyValueBuildCode);
                else
                    return uri.Combine(UrlCity)
                        .Combine(keyValueBuildingType, keyValueBuildCode);
            }
        }
    }
}
