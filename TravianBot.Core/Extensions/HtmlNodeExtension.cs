using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravianBot.Core.Extensions
{
    static class HtmlNodeExtension
    {
        public static bool HasAttributeAndContainsValue(this HtmlNode node, string attri, string value)
        {
            return node.Attributes.Contains(attri) && node.Attributes[attri].Value.Contains(value);
        }

        public static bool HasAttributeAndNotContainsValue(this HtmlNode node, string attri, string value)
        {
            return node.Attributes.Contains(attri) && !node.Attributes[attri].Value.Contains(value);
        }

        public static bool HasClass(this HtmlNode node, string classValue)
        {
            var values = classValue.Split(' ');
            foreach (var value in values)
            {
                if (node.Attributes.Contains("class") && node.Attributes["class"].Value.Contains(value))
                    return true;
            }
            return false;
        }

        public static bool HasClassExcept(this HtmlNode node, string classValue)
        {
            var values = classValue.Split(' ');
            foreach (var value in values)
            {
                if (node.Attributes.Contains("class") && !node.Attributes["class"].Value.Contains(value))
                    return true;
            }
            return false;
        }
    }
}
