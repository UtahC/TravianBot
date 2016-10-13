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
            var oldvillage = new Village()
            {
                VillageId = 1,
                IsActive = true,
                Buildings = null,
                IsCapital = false,
                X = 10,
                Y = 11,
                VillageName = "name",
            };
            var newVillage = new Village()
            {
                VillageId = 1,
                IsActive = false,
                IsCapital = true,
                Buildings = null,
                X = 20,
                Y = 21,
                VillageName = "new name",
            };

            oldvillage.UpdatePropertyIfNotEquals(v => v.VillageId, newVillage.VillageId);
            oldvillage.UpdatePropertyIfNotEquals(v => v.IsActive, newVillage.IsActive);
            oldvillage.UpdatePropertyIfNotEquals(v => v.Buildings, newVillage.Buildings);
            oldvillage.UpdatePropertyIfNotEquals(v => v.IsCapital, newVillage.IsCapital);
            oldvillage.UpdatePropertyIfNotEquals(v => v.X, newVillage.X);
            oldvillage.UpdatePropertyIfNotEquals(v => v.Y, newVillage.Y);
            oldvillage.UpdatePropertyIfNotEquals(v => v.VillageName, newVillage.VillageName);

            Assert.AreEqual(oldvillage, newVillage);

        }
    }
}