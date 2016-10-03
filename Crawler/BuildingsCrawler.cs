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
                int[] nums = {1, 34, 30, 31, 32, 33, 40 ,26};
                for (int i = 1; i <= 42; i++)
                {
                    if (!nums.Contains(i))
                        continue;

                    string url = $"https://travian.kirilloid.ru/build.php#b={i}&mb=1&s=1.44";
                    driver.Navigate().GoToUrl(url);
                    driver.Navigate().Refresh();
                    
                    var doc = new HtmlDocument();
                    doc.LoadHtml(driver.PageSource);

                    var info = new BuildingInfo();
                    info.Name = doc.GetElementbyId("data_holder-title").InnerHtml;
                    info.Id = i;
                    
                    if (!doc.GetElementbyId("data_holder-req").InnerHtml.Contains("none"))
                    {
                        info.Prerequiresites = new List<Prerequiresite>();
                        var reqNodes = doc.GetElementbyId("data_holder-req").ChildNodes;
                        foreach (var reqNode in reqNodes)
                        {
                            var req = new Prerequiresite();
                            info.NeedCapital = false;
                            info.NeedNotCapital = false;
                            info.TribeRequired = Tribes.None;

                            if (reqNode.Name == "strike" && reqNode.FirstChild.Name == "a")
                            {
                                req.Id = reqNode.FirstChild.Attributes["onclick"].Value.GetInt();
                                req.Name = reqNode.FirstChild.LastChild.InnerText.Trim();
                                req.Level = -1;
                            }
                            if (reqNode.Name == "a")
                            {
                                req.Id = reqNode.Attributes["onclick"].Value.GetInt();
                                req.Name = reqNode.LastChild.InnerText.Trim();
                                req.Level = reqNode.NextSibling.InnerHtml.GetInt();
                            }
                            if (reqNode.InnerText.ToLower().Contains("capital"))
                            {
                                if (reqNode.AncestorsAndSelf("strike").Count() <= 0)
                                    info.NeedCapital = true;
                                else
                                    info.NeedNotCapital = true;
                            }
                            if (reqNode.InnerText.ToLower().Contains("tribes"))
                            {
                                if (reqNode.InnerText.ToLower().Contains("romans"))
                                    info.TribeRequired = Tribes.Romans;
                                else if (reqNode.InnerText.ToLower().Contains("teutons"))
                                    info.TribeRequired = Tribes.Teutons;
                                else if (reqNode.InnerText.ToLower().Contains("gauls"))
                                    info.TribeRequired = Tribes.Gauls;
                            }
                            
                            if (req.Id > 0)
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
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Prerequiresite> Prerequiresites { get; set; }
        public List<BuildingUpgradeInfo> Details { get; set; }
        public bool NeedCapital { get; set; }
        public bool NeedNotCapital { get; set; }
        public Tribes TribeRequired { get; set; }
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

    enum Tribes
    {
        None,
        Romans,
        Teutons,
        Gauls,
    }
}
