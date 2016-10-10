using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravianBot.Core.Tasks
{
    class CheckTask : TaskBase
    {
        public static bool IsLogon()
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(client.Html);
            if (doc.GetElementbyId("sidebarBoxVillagelist") == null)
                return false;
            return true;
        }
    }
}
