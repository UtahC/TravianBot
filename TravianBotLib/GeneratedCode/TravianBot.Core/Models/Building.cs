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
	using TravianBot.Core.Enums;

	public class Building
	{
		public virtual int Id
		{
			get;
			set;
		}

		public virtual int VillageId
		{
			get;
			set;
		}

		public virtual object Level
		{
			get;
			set;
		}

		public virtual Buildings Type
		{
			get;
			set;
		}

	}
}
