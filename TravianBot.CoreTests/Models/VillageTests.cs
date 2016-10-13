using Microsoft.VisualStudio.TestTools.UnitTesting;
using TravianBot.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravianBot.Core.Models.Tests
{
    [TestClass()]
    public class VillageTests
    {
        [TestMethod()]
        public void UpdatePropertyTest()
        {
            var oldvillage = new Village(1, "name", 10, 11, true, false);
            var newVillage = new Village(1, "new name", 20, 21, false, true);

            oldvillage.UpdatePropertyIfNotEquals(v => v.VillageId, newVillage.VillageId);
            oldvillage.UpdatePropertyIfNotEquals(v => v.IsActive, newVillage.IsActive);
            oldvillage.UpdatePropertyIfNotEquals(v => v.IsCapital, newVillage.IsCapital);
            oldvillage.UpdatePropertyIfNotEquals(v => v.X, newVillage.X);
            oldvillage.UpdatePropertyIfNotEquals(v => v.Y, newVillage.Y);
            oldvillage.UpdatePropertyIfNotEquals(v => v.VillageName, newVillage.VillageName);

            Assert.AreEqual(oldvillage, newVillage);

        }
    }
}