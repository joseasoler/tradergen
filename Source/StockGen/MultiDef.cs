using System.Collections.Generic;
using System.Text;
using RimWorld;
using Verse;

namespace TG.StockGen
{
	/// <summary>
	/// Chooses from a list of MultiDefs. Vanilla MultiDef lacks a thingDefCountRange attribute and therefore always
	/// generates a single random ThingDef.
	/// </summary>
	public class MultiDef : ConditionMatcher
	{
		public List<ThingDef> thingDefs = new List<ThingDef>();

		public override void ConditionToText(ref StringBuilder b)
		{
			base.ConditionToText(ref b);
			Util.ToText(ref b, "thingDefs", thingDefs);
		}

		protected override bool CanBuy(in ThingDef def)
		{
			return thingDefs.Contains(def);
		}
	}
}