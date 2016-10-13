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
                new Village(1, "name1", 10, 11, true, false),
                new Village(2, "name2", 20, 21, false, true)
            });
            newVillages = newVillages = new List<Village>()
            {
                new Village(1, "name1", 10, 11, true, false),
                new Village(2, "name2", 20, 21, false, true)
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
            newVillages.Add(new Village(33, "name3", 30, 31, true, true));
            UtilityTask.LoadVillages(villages, newVillages);

            Assert.AreEqual(AreTwoVillagesEqual(), true);
        }

        [TestMethod()]
        public void LoadVillagesUpdateTest()
        {
            newVillages[1] = new Village(2, "name22", 20, 21, true, true);
            UtilityTask.LoadVillages(villages, newVillages);

            Assert.AreEqual(AreTwoVillagesEqual(), true);
        }

        [TestMethod()]
        public void LoadVillagesAllTest()
        {
            newVillages.RemoveAt(1);
            newVillages.Add(new Village(33, "name3", 30, 31, true, true));
            newVillages[0] = new Village(11, "name11", 11, 12, true, false);
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