using System.Collections.Generic;
using System.Linq;
using Verse;

namespace TG.StockGen
{
	/// <summary>
	/// Purchase anything with a high enough market value per unit of volume.
	/// Fulfills a similar role than StockGenerator_BuyExpensiveSimple, but it ignores additional objects such as
	/// edible items or relics
	/// </summary>
	public class ExpensiveSimple : ConditionMatcher
	{
		/// <summary>
		/// Minimum value per unit of volume.
		/// </summary>
		public float minValuePerUnit = 10f;

		private static HashSet<ThingDef> _disallowedThings;

		protected override bool CanBuy(in ThingDef def)
		{
			if (_disallowedThings == null)
			{
				_disallowedThings = new HashSet<ThingDef>();
				_disallowedThings.AddRange(DefOf.ThingCategory.BodyPartsNatural.DescendantThingDefs);
				_disallowedThings.AddRange(DefOf.ThingCategory.EggsFertilized.DescendantThingDefs);
				_disallowedThings.AddRange(DefOf.ThingCategory.EggsUnfertilized.DescendantThingDefs);
				_disallowedThings.AddRange(DefOf.ThingCategory.InertRelics.DescendantThingDefs);
			}

			return def.category == ThingCategory.Item && !def.IsApparel && !def.IsWeapon && !def.IsMedicine && !def.IsDrug &&
			       !_disallowedThings.Contains(def) &&
			       def.BaseMarketValue / def.VolumePerUnit >= minValuePerUnit;
		}
	}
}