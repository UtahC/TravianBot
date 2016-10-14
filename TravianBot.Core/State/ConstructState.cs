using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravianBot.Core.Enums;
using TravianBot.Core.Extensions;

namespace TravianBot.Core.State
{
    class ConstructState : StateBase
    {
        public void Build(bool isZeroLevel, int villageId, int buildingId, Buildings type)
        {
            client.LoadUrl(UrlGenerator.GetBuildingUri(villageId, buildingId));
            var buildCode = GetBuildCode();
            
            if (buildCode != null)
                client.LoadUrl(UrlGenerator.GetExecuteBuildUri(isZeroLevel, type, buildingId, buildCode));
        }

        public string GetBuildCode()
        {
            var doc = client.Document;
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
