using System.Collections.Generic;
using System.Linq;
using Verse;

namespace TG.StockGen
{
	/// <summary>
	/// Handles anything with a high enough market value per unit of volume.
	/// Intended to replace StockGenerator_BuyExpensiveSimple. It ignores additional objects such as organs, eggs or
	/// relics. If needed it can also be used to generate random, high-value stock.
	/// </summary>
	public class ExpensiveSimple : ConditionMatcher
	{
		/// <summary>
		/// Minimum value per unit of volume.
		/// </summary>
		public float minValuePerUnit = 10f;

		/// <summary>
		/// Lazily initialized cache of disallowed things. They belong to one of the categories in CanBuy.
		/// </summary>
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