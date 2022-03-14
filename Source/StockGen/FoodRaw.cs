using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using ThingCategory = TG.DefOf.ThingCategory;

namespace TG.StockGen
{
	/// <summary>
	/// Matches any raw food.
	/// Currently it is extremely similar to StockGenerator_Category with FoodRaw.
	/// Excludes canned food as it is not meant to be sold by non-orbital traders. Orbital traders handle it separately.
	/// It is planned to update this StockGenerator to generate faction and/or ideology appropriate food in the future.
	/// Currently follows a "default" ideology and excludes insect and human meat.
	/// </summary>
	public class FoodRaw : ConditionMatcher
	{
		protected override bool CanBuy(in ThingDef def)
		{
			var meatSourceCategory = FoodUtility.GetMeatSourceCategory(def);
			return def.IsIngestible && ThingCategory.FoodRaw.DescendantThingDefs.Contains(def) &&
			       !def.thingCategories.Contains(ThingCategoryDefOf.EggsFertilized) &&
			       meatSourceCategory != MeatSourceCategory.Humanlike &&
			       meatSourceCategory != MeatSourceCategory.Insect &&
			       // Exclude fermented rotting mound from Alpha Animals. Also alcohol, which is handled separately.
			       !def.ingestible.foodType.HasFlag(FoodTypeFlags.Liquor);
		}

		protected override bool CanSell(in ThingDef def, in int forTile, in Faction faction)
		{
			return CanBuy(def) &&
			       // Exclude canned food from Vanilla Cooking Expanded.
			       (def.ingestible.tasteThought == null || def.ingestible.tasteThought.defName != "VCE_ConsumedCannedGoods");
		}
	}
}