using Microsoft.VisualStudio.TestTools.UnitTesting;
using TravianBot.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinqToDB;
using TravianBot.Core.Models;
using System.IO;

namespace TravianBot.Core.Tests
{
    [TestClass()]
    public class ClientTests
    {
        static Client client = Client.Default;

        [TestInitialize]
        public void Init()
        {
            string from = @"C:\Users\UtahC\Documents\Visual Studio 2015\Projects\TravianBot\TravianBot.Core\TravianBot.mdb";
            string to = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TravianBot.mdb");
            File.Delete(to);
            File.Copy(from, to);
            var village = new Village(1, "name1", 1, 1, true, true);
            client.Villages.Add(village);
        }

        [TestMethod()]
        public void VillageInsertTest()
        {
            var village = new Village(11, "name11", 11, 12, true, true);
            client.Villages.Add(village);

            using (var db = new TravianBotDB())
            {
                var q = from c in db.DB_Villages where c.VillageId == 11 select c;
                Assert.AreEqual(village.X, q.FirstOrDefault().X);
                Assert.AreEqual(village.Y, q.FirstOrDefault().Y);
                Assert.AreEqual(village.VillageName, q.FirstOrDefault().VillageName);
                Assert.AreEqual(village.IsCapital, q.FirstOrDefault().IsCapital);
            }
        }

        [TestMethod()]
        public void VillageDeleteTest()
        {
            client.Villages.RemoveAt(0);

            using (var db = new TravianBotDB())
            {
                var q = from c in db.DB_Villages where c.VillageId == 1 select c;
                
                Assert.AreEqual(0, q.Count());
            }
        }

        [TestMethod()]
        public void VillageUpdateTest()
        {
            client.Villages.FirstOrDefault().IsCapital = false;

            using (var db = new TravianBotDB())
            {
                var q = from c in db.DB_Villages where c.VillageId == 1 select c;

                Assert.AreEqual(false, q.FirstOrDefault().IsCapital);
            }
        }
    }
}