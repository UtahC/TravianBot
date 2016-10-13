using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravianBot.Core
{
    class ScriptGenerator
    {
        public static string GetLoginScript(string account, string password, bool isLowQuality)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"document.getElementsByName('name')[0].value = '{account}';");
            sb.Append($"document.getElementsByName('password')[0].value = '{password}';");
            if (isLowQuality)
                sb.Append("document.getElementById('lowRes').checked=true;");
            else
                sb.Append("document.getElementById('lowRes').checked=false;");
            sb.Append("document.getElementById('s1').click();");

            return sb.ToString();
        }
    }
}
