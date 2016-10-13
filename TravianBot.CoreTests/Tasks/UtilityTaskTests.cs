using Microsoft.VisualStudio.TestTools.UnitTesting;
using TravianBot.Core.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravianBot.Core.Models;
using System.Collections.ObjectModel;

namespace TravianBot.Core.Tasks.Tests
{
    [TestClass()]
    public class UtilityTaskTests
    {
        ObservableCollection<Village> villages;
        List<Village> newVillages;

        [TestInitialize()]
        public void InitTowVillages()
        {
            villages = new ObservableCollection<Village>(new List<Village>()
            {
                new Village()
                {
                    VillageId = 1,
                    IsActive = true,
                    Buildings = null,
                    IsCapital = false,
                    X = 10,
                    Y = 11,
                    VillageName = "name1"
                },
                new Village()
                {
                    VillageId = 2,
                    IsActive = false,
                    Buildings = null,
                    IsCapital = true,
                    X = 20,
                    Y = 21,
                    VillageName = "name2"
                }
            });
            newVillages = newVillages = new List<Village>()
            {
                new Village()
                {
                    VillageId = 1,
                    IsActive = true,
                    Buildings = null,
                    IsCapital = false,
                    X = 10,
                    Y = 11,
                    VillageName = "name1"
                },
                new Village()
                {
                    VillageId = 2,
                    IsActive = false,
                    Buildings = null,
                    IsCapital = true,
                    X = 20,
                    Y = 21,
                    VillageName = "name2"
                }
            };
        }

        [TestMethod()]
        public void LoadVillagesNoneTest()
        {
            UtilityTask.LoadVillages(villages, newVillages);
            
            Assert.AreEqual(AreTwoVillagesEqual(), true);
        }

        [TestMethod()]
        public void LoadVillagesRemoveTest()
        {
            newVillages.RemoveAt(0);
            UtilityTask.LoadVillages(villages, newVillages);

            Assert.AreEqual(AreTwoVillagesEqual(), true);
        }

        [TestMethod()]
        public void LoadVillagesAddTest()
        {
            newVillages.Add(new Village()
            {
                VillageId = 33,
                IsActive = true,
                Buildings = null,
                IsCapital = true,
                X = 30,
                Y = 31,
                VillageName = "name3",
            });
            UtilityTask.LoadVillages(villages, newVillages);

            Assert.AreEqual(AreTwoVillagesEqual(), true);
        }

        [TestMethod()]
        public void LoadVillagesUpdateTest()
        {
            newVillages[1] = new Village()
            {
                VillageId = 2,
                IsActive = true,
                Buildings = null,
                IsCapital = true,
                X = 20,
                Y = 21,
                VillageName = "name22"
            };
            UtilityTask.LoadVillages(villages, newVillages);

            Assert.AreEqual(AreTwoVillagesEqual(), true);
        }

        [TestMethod()]
        public void LoadVillagesAllTest()
        {
            newVillages.RemoveAt(1);
            newVillages.Add(new Village()
            {
                VillageId = 33,
                IsActive = true,
                Buildings = null,
                IsCapital = true,
                X = 30,
                Y = 31,
                VillageName = "name3",
            });
            newVillages[0] = new Village()
            {
                VillageId = 11,
                IsActive = true,
                Buildings = null,
                IsCapital = false,
                X = 11,
                Y = 12,
                VillageName = "name11"
            };
            UtilityTask.LoadVillages(villages, newVillages);

            Assert.AreEqual(AreTwoVillagesEqual(), true);
        }

        private bool AreTwoVillagesEqual()
        {
            var list1 = villages.OrderBy(v => v.VillageName).ToList();
            var list2 = newVillages.OrderBy(v => v.VillageName).ToList();

            if (list1.Count != list2.Count)
                return false;
            for (int i = 0; i < list1.Count; i++)
            {
                if (!list1[i].Equals(list2[i]))
                    return false;
            }

            return true;
        }
    }
}