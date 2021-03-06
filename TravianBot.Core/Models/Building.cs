﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool
//     Changes to this file will be lost if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
namespace TravianBot.Core.Models
{
    using GalaSoft.MvvmLight;
    using LinqToDB;
    using LinqToDB.Mapping;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using TravianBot.Core.Enums;
    using TravianBot.Core.Information;

    public class Building : ObservableObject
    {
        private int buildingId;
        private int villageId;
        private int level;
        private Buildings buildingType;
        //private string typeString;
        private BuildingInfo info;
        
		public  int BuildingId
		{
            get
            {
                return buildingId;
            }
            set
            {
                Set(() => BuildingId, ref buildingId, value);
            }
        }
		public  int VillageId
		{
            get
            {
                return villageId;
            }
            set
            {
                Set(() => VillageId, ref villageId, value);
            }
        }
		public  int Level
		{
            get
            {
                return level;
            }
            set
            {
                Set(() => Level, ref level, value);
            }
        }
        public Buildings BuildingType
		{
            get
            {
                return buildingType;
            }
            set
            {
                Set(() => BuildingType, ref buildingType, value);
            }
        }
		public BuildingInfo Info
		{
            get
            {
                return info;
            }
            set
            {
                Set(() => Info, ref info, value);
            }
        }

        public Building()
        {
            //PropertyChanged += (s, e) =>
            //{
            //    Task.Run(() =>
            //    {
            //        if (e.PropertyName == "BuildingType")
            //            TypeString = buildingType.ToString();
            //        using (var db = new TravianBotDB())
            //        {
            //            //db.DB_Buildings.Where(dB => dB.VillageId == VillageId && dB.BuildingId == BuildingId)
            //            //.Set(dB => dB.Level, Level)
            //            //.Set(dB => dB.BuildingId, BuildingId)
            //        }
            //    });
            //};
        }

        //public override bool Equals(object obj)
        //{
        //    var that = obj as Building;
        //    if (that == null)
        //        return false;

        //}
    }

    //[Table("DB_Buildings")]
    //public partial class DB_Building : ObservableObject
    //{
    //    [PrimaryKey, Identity]
    //    public virtual int DB_Id { get; set; } // Long
    //    [Column, Nullable]
    //    public virtual int BuildingId { get; set; } // Long
    //    [Column, Nullable]
    //    public virtual int VillageId { get; set; } // Long
    //    [Column, Nullable]
    //    public virtual int Level { get; set; } // Long
    //    [Column, Nullable]
    //    public virtual string TypeString { get; set; } // text(255)
    //}
}

