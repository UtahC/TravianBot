﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool
//     Changes to this file will be lost if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
namespace TravianBot.Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Enums;
    using System.IO;
    using Newtonsoft.Json;

    public interface ISetting
    {
        string Account { get; set; }

        string Server { get; set; }

        string Password { get; set; }

        Speeds Speed { get; set; }

        bool IsLowResolution { get; set; }

        UserAgents UserAgent { get; set; }

        string UserAgentString { get;}

        bool IsUseProxy { get; set; }

        string ProxyString { get; }

        string ProxyHost { get; set; }

        string ProxyPort { get; set; }

        string ProxyLogin { get; set; }

        string ProxyPassword { get; set; }

        Tribes Tribe { get; set; }

        LogicSetting LogicSetting { get; set; }

        void Save();

    }

    public class Setting : ISetting
	{
        private static readonly string settingFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "setting.json");
        private static Setting setting;
        private Uri server;

        public static Setting Default
        {
            get
            {
                if (setting == null)
                    setting = Load();
                return setting;
            }
        }

        public string Account { get; set; } = "";

        public string Server
        {
            get
            {
                return server == null ? "" : server.AbsoluteUri;
            }
            set
            {
                Uri result;
                if (Uri.TryCreate(value, UriKind.Absolute, out result))
                    server = result;
            }
        }

        public string Password { get; set; } = "";

        public Speeds Speed { get; set; } = Speeds.x1;

        public bool IsLowResolution { get; set; } = false;

        public UserAgents UserAgent { get; set; } = UserAgents.Default;

        public string UserAgentString { get { return GetUserAgentString(UserAgent); } }

        public bool IsUseProxy { get; set; } = false;

        [JsonIgnore]
        public string ProxyString { get { return $"{ProxyHost}:{ProxyPort}"; } }

        public string ProxyHost { get; set; } = "";

        public string ProxyPort { get; set; } = "";

        public string ProxyLogin { get; set; } = "";

        public string ProxyPassword { get; set; } = "";

        public Tribes Tribe { get; set; } = Tribes.None;

        public LogicSetting LogicSetting { get; set; }

        private Setting()
        {

        }

        public void Save()
		{
            File.WriteAllText(settingFilePath, JsonConvert.SerializeObject(this));
            Client.Default.Logger.Write("Account settings saved.");
        }

        private static Setting Load()
        {
            if (!File.Exists(settingFilePath))
            {
                File.WriteAllText(settingFilePath, JsonConvert.SerializeObject(new Setting()));
            }

            return JsonConvert.DeserializeObject<Setting>(File.ReadAllText(settingFilePath));
        }

        private static string GetUserAgentString(UserAgents userAgent)
        {
            #region UserAgent string array
            string[] userAgentString =
            {
                "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1)",
                "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.0)",
                "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.1; Trident/4.0; SLCC1; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729; .NET CLR 1.1.4322)",
                "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.0; Trident/4.0; SLCC1; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729; .NET CLR 1.1.4322)",
                "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; Trident/4.0; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729)",
                "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; WOW64; Trident/4.0; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729)",
                "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.0; Trident/5.0; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729)",
                "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729)",
                "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729)",
                "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; Trident/6.0; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729)",
                "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; WOW64; Trident/6.0; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729)",
                "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; Trident/6.0; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729)",
                "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729)",
                "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.3; Trident/6.0; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729)",
                "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.3; WOW64; Trident/6.0; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729)",
                "Mozilla/5.0 (Windows NT 6.1; Trident/7.0; rv:11.0; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729) like Gecko",
                "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729) like Gecko",
                "Mozilla/5.0 (Windows NT 6.2; Trident/7.0; rv:11.0; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729) like Gecko",
                "Mozilla/5.0 (Windows NT 6.2; WOW64; Trident/7.0; rv:11.0; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729) like Gecko",
                "Mozilla/5.0 (Windows NT 6.3; Trident/7.0; rv:11.0; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729) like Gecko",
                "Mozilla/5.0 (Windows NT 6.3; WOW64; Trident/7.0; rv:11.0; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729) like Gecko",
                "Mozilla/5.0 (Windows; U; Windows NT 5.1; ru; rv:1.9.1.2) Gecko/20090729 Firefox/3.5.2 (.NET CLR 3.5.30729)",
                "Mozilla/5.0 (Windows; U; Windows NT 6.0; ru; rv:1.9.1.2) Gecko/20090729 Firefox/3.5.2 (.NET CLR 3.5.30729)",
                "Mozilla/5.0 (Windows; U; Windows NT 6.1; ru; rv:1.9.1.2) Gecko/20090729 Firefox/3.5.2 (.NET CLR 3.5.30729)",
                "Mozilla/5.0 (Windows NT 5.1; rv:12.0) Gecko/20120403211507 Firefox/14.0.1",
                "Mozilla/5.0 (Windows NT 6.0; rv:12.0) Gecko/20120403211507 Firefox/14.0.1",
                "Mozilla/5.0 (Windows NT 6.1; rv:12.0) Gecko/20120403211507 Firefox/14.0.1",
                "Mozilla/5.0 (Windows NT 6.2; rv:12.0) Gecko/20120403211507 Firefox/14.0.1",
                "Mozilla/5.0 (Windows NT 6.3; rv:12.0) Gecko/20120403211507 Firefox/14.0.1",
                "Mozilla/5.0 (Windows NT 6.0; rv:25.0) Gecko/20100101 Firefox/25.0",
                "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:25.0) Gecko/20100101 Firefox/25.0",
                "Mozilla/5.0 (Windows NT 6.2; Win64; x64; rv:25.0) Gecko/20100101 Firefox/25.0",
                "Mozilla/5.0 (Windows NT 6.3; Win64; x64; rv:25.0) Gecko/20100101 Firefox/25.0",
                "Mozilla/5.0 (Windows NT 6.0; rv:25.0) Gecko/20100101 Firefox/27.0",
                "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:25.0) Gecko/20100101 Firefox/27.0",
                "Mozilla/5.0 (Windows NT 6.2; Win64; x64; rv:25.0) Gecko/20100101 Firefox/27.0",
                "Mozilla/5.0 (Windows NT 6.3; Win64; x64; rv:25.0) Gecko/20100101 Firefox/27.0",
                "Opera/9.80 (Windows NT 5.1; U; ru) Presto/2.2.15 Version/10.10",
                "Opera/9.80 (Windows NT 6.0; U; ru) Presto/2.2.15 Version/10.10",
                "Opera/9.80 (Windows NT 6.1; U; ru) Presto/2.2.15 Version/10.10",
                "Opera/9.80 (Windows NT 5.1; U; ru) Presto/2.10.229 Version/11.62",
                "Opera/9.80 (Windows NT 6.0; U; ru) Presto/2.10.229 Version/11.62",
                "Opera/9.80 (Windows NT 6.1; WOW64; U; ru) Presto/2.10.229 Version/11.62",
                "Opera/9.80 (Windows NT 6.2; WOW64; U; ru) Presto/2.10.229 Version/11.62",
                "Opera/9.80 (Windows NT 5.1; U; ru) Presto/2.12.388 Version/12.14",
                "Opera/9.80 (Windows NT 6.0; U; ru) Presto/2.12.388 Version/12.14",
                "Opera/9.80 (Windows NT 6.1; WOW64; U; ru) Presto/2.12.388 Version/12.14",
                "Opera/9.80 (Windows NT 6.2; WOW64; U; ru) Presto/2.12.388 Version/12.14",
                "Opera/9.80 (Windows NT 6.3; WOW64; U; ru) Presto/2.12.388 Version/12.14",
                "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US) AppleWebKit/534.16 (KHTML, like Gecko) Chrome/10.0.648.204 Safari/534.16",
                "Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US) AppleWebKit/534.16 (KHTML, like Gecko) Chrome/10.0.648.204 Safari/534.16",
                "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/534.16 (KHTML, like Gecko) Chrome/10.0.648.204 Safari/534.16",
                "Mozilla/5.0 (Windows NT 5.1) AppleWebKit/537.13 (KHTML, like Gecko) Chrome/24.0.1290.1 Safari/537.13",
                "Mozilla/5.0 (Windows NT 6.0) AppleWebKit/537.13 (KHTML, like Gecko) Chrome/24.0.1290.1 Safari/537.13",
                "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.13 (KHTML, like Gecko) Chrome/24.0.1290.1 Safari/537.13",
                "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.13 (KHTML, like Gecko) Chrome/24.0.1290.1 Safari/537.13",
                "Mozilla/5.0 (Windows NT 5.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/32.0.1667.0 Safari/537.36",
                "Mozilla/5.0 (Windows NT 6.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/32.0.1667.0 Safari/537.36",
                "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/32.0.1667.0 Safari/537.36",
                "Mozilla/5.0 (Windows NT 6.2; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/32.0.1667.0 Safari/537.36",
                "Mozilla/5.0 (Windows NT 6.3; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/32.0.1667.0 Safari/537.36"
            };
            #endregion
            return (int)userAgent == 0 ? "" : userAgentString[(int)userAgent - 1];
        }
    }
    
    public class LogicSetting
    {
        public IEnumerable<BuildingTaskModel> BuildingTasks { get; set; }
    }
}

