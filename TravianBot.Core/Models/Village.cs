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
	using TravianBot.Core;

	public class Village : GalaSoft.MvvmLight.ObservableObject
    {
        private int id;
        private string name;
        private int x;
        private int y;
        private bool isActive;
        private bool isCapital;
        private IEnumerable<Building> buildings;

        public int Id
		{
            get
            {
                return id;
            }
            set
            {
                Set(() => Id, ref id, value);
            }
		}

		public string Name
		{
            get
            {
                return name;
            }
            set
            {
                Set(() => Name, ref name, value);
            }
        }

		public int X
		{
            get
            {
                return x;
            }
            set
            {
                Set(() => X, ref x, value);
            }
        }

		public int Y
		{
            get
            {
                return y;
            }
            set
            {
                Set(() => Y, ref y, value);
            }
        }

		public bool IsActive
		{
            get
            {
                return isActive;
            }
            set
            {
                Set(() => IsActive, ref isActive, value);
            }
        }

		public bool IsCapital
		{
            get
            {
                return isCapital;
            }
            set
            {
                Set(() => IsCapital, ref isCapital, value);
            }
        }

		public IEnumerable<Building> Buildings
		{
            get
            {
                return buildings;
            }
            set
            {
                Set(() => Buildings, ref buildings, value);
            }
        }

	}
}
