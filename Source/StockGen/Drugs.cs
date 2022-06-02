using RimWorld;
using Verse;

namespace TG.StockGen
{
	/// <summary>
	/// Generates drugs. Alcoholic and non-alcoholic beverages are ignored as they have specific StockGens.
	/// </summary>
	public class Drugs : ConditionMatcher
	{
		/// <summary>
		/// If set to a specific value, it will only generate drugs from the chosen category.
		/// </summary>
		public DrugCategory drugCategory = DrugCategory.Any;

		protected override bool CanBuy(in ThingDef def)
		{
			return def.IsWithinCategory(ThingCategoryDefOf.Drugs) && !Things.Util.IsAlcohol(def) &&
			       (def.thingCategories == null ||
			        !def.thingCategories.Any(cat => cat.defName == "VBE_DrinksNonAlcoholic")) &&
			       def.IsIngestible && (drugCategory == DrugCategory.Any || def.ingestible.drugCategory == drugCategory);
		}
	}
}