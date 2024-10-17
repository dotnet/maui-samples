using System;
using System.ComponentModel;
using System.Reflection;

namespace PointOfSale.Models
{
	public enum OrderType
	{
		[Description("Dine In")]
		DineIn,
        [Description("Carry Out")]
        CarryOut,
        [Description("Delivery")]
        Delivery
	}
}

