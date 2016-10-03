using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using System.Diagnostics;
using OpenQA.Selenium.Remote;

namespace Crawler
{
    static class StringExtension
    {
        public static int GetInt(this string str)
        {
            return int.Parse(string.Concat(str.Where(c => c >= 48 && c <= 57)));
        }
    }

    static class HtmlNodeExtension
    {
        public static bool HasClass(this HtmlNode node, string attri)
        {
            return node.Attributes.Contains("class") && node.Attributes["class"].Value.Contains(attri);
        }
    }

    class BuildingsCrawler
    {
        public void Excute()
        {
            var infos = new List<BuildingInfo>();

            using (var driver = new ChromeDriver())
            {
                for (int i = 1; i <= 42; i++)
                {
                    string url = $"https://travian.kirilloid.ru/build.php#b={i}&mb=1&s=1.44";
                    driver.Navigate().GoToUrl(url);
                    driver.Navigate().Refresh();
                    
                    var doc = new HtmlDocument();
                    doc.LoadHtml(driver.PageSource);

                    var info = new BuildingInfo();
                    info.Name = doc.GetElementbyId("data_holder-title").InnerHtml;

                    var reqNodes = doc.GetElementbyId("data_holder-req").Descendants("a");
                    if (reqNodes.Count() > 0)
                    {
                        info.Prerequiresites = new List<Prerequiresite>();
                        foreach (var reqNode in reqNodes)
                        {
                            var req = new Prerequiresite();
                            req.Id = reqNode.Attributes["onclick"].Value.GetInt();
                            req.Name = reqNode.LastChild.InnerText.Trim();
                            if (reqNode.ParentNode.Name == "strike")
                                req.Level = -1;
                            else
                                req.Level = reqNode.NextSibling.InnerHtml.GetInt();
                            info.Prerequiresites.Add(req);
                        }
                    }

                    var theadNode = doc.GetElementbyId("data").ChildNodes["thead"];
                    var tbodyNode = doc.GetElementbyId("data").ChildNodes["tbody"];
                    var additionalInfoName = theadNode.Descendants("td").LastOrDefault().InnerText;
                    info.Details = new List<BuildingUpgradeInfo>();
                    foreach (var rowNode in tbodyNode.ChildNodes.Where(n => n.NodeType == HtmlNodeType.Element && !n.HasClass("sum")))
                    {
                        var detail = new BuildingUpgradeInfo();
                        detail.Level = int.Parse(rowNode.ChildNodes[0].InnerText);
                        detail.WoodCost = int.Parse(rowNode.ChildNodes[1].InnerText);
                        detail.ClayCost = int.Parse(rowNode.ChildNodes[2].InnerText);
                        detail.IronCost = int.Parse(rowNode.ChildNodes[3].InnerText);
                        detail.CropCost = int.Parse(rowNode.ChildNodes[4].InnerText);
                        detail.TotalCost = int.Parse(rowNode.ChildNodes[5].InnerText);
                        detail.PopulationSpan = int.Parse(rowNode.ChildNodes[6].InnerText);
                        detail.CulturePoint = int.Parse(rowNode.ChildNodes[9].InnerText);
                        detail.TimeCost = rowNode.ChildNodes[10].InnerText;
                        detail.AdditionalInfo = new KeyValuePair<string, string>(additionalInfoName, rowNode.ChildNodes[11].InnerText);
                        info.Details.Add(detail);
                    }
                    infos.Add(info);
                }
            }
            Debug.WriteLine(JsonConvert.SerializeObject(infos));
        }
    }

    class BuildingInfo
    {
        public string Name { get; set; }
        public List<Prerequiresite> Prerequiresites { get; set; }
        public List<BuildingUpgradeInfo> Details { get; set; }
    }

    class Prerequiresite
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
    }

    class BuildingUpgradeInfo
    {
        public int Level { get; set; }
        public int WoodCost { get; set; }
        public int ClayCost { get; set; }
        public int IronCost { get; set; }
        public int CropCost { get; set; }
        public int TotalCost { get; set; }
        public int PopulationSpan { get; set; }
        public int CulturePoint { get; set; }
        public string TimeCost { get; set; }
        public KeyValuePair<string, string> AdditionalInfo { get; set; }
    }
}
