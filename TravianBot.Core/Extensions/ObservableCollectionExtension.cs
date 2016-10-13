using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TravianBot.Core.Models;

namespace TravianBot.Core.Extensions
{
    static class ObservableCollectionExtension
    {
        public static void UpdateVillageById(this ObservableCollection<Village> villages, Village newVillage)
        {
            var oldVillage = villages.Where(v => v.VillageId == newVillage.VillageId).FirstOrDefault();
            if (oldVillage == null)
                throw new Exception($"Village not found.");

            oldVillage.UpdatePropertyIfNotEquals(v => v.VillageName, newVillage.VillageName);
            oldVillage.UpdatePropertyIfNotEquals(v => v.IsActive, newVillage.IsActive);
            oldVillage.UpdatePropertyIfNotEquals(v => v.IsCapital, newVillage.IsCapital);
        }
    }
}
