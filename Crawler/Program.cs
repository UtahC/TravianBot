﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawler
{
    class Program
    {
        static void Main(string[] args)
        {
            var crawlerBuildings = new BuildingsCrawler();
            crawlerBuildings.Excute();
        }
    }
}
