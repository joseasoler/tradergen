using RimWorld;
using Verse;

namespace TG.StockGen
{
	/// <summary>
	/// Generates drugs. Alcoholic and non-alcoholic beverages are ignored as they have specific StockGens.
	/// </summary>
	public class Drugs : ConditionMatcher
	{
		protected override bool CanBuy(in ThingDef def)
		{
			return def.IsWithinCategory(ThingCategoryDefOf.Drugs) && !Util.IsAlcohol(def) && (def.thingCategories == null ||
				!def.thingCategories.Any(cat => cat.defName == "VBE_DrinksNonAlcoholic"));
		}
	}
}