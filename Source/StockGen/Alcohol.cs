using System.Linq;
using RimWorld;
using TG.DefOf;
using Verse;

namespace TG.StockGen
{
	/// <summary>
	/// Handles any alcoholic beverage, while excluding non-alcoholic beverages and rotting mounds from mods.
	/// </summary>
	public class Alcohol : ConditionMatcher
	{
		protected override bool CanBuy(in ThingDef def)
		{
			if (def.ingestible?.outcomeDoers == null || def.ingestible.JoyKind != JoyKind.Chemical ||
			    !def.ingestible.foodType.HasFlag(FoodTypeFlags.Liquor)) return false;

			return (from outcomeDoer in def.ingestible.outcomeDoers
				where outcomeDoer.GetType() == typeof(IngestionOutcomeDoer_GiveHediff)
				select (IngestionOutcomeDoer_GiveHediff) outcomeDoer).Any(o => o.toleranceChemical == ChemicalDefOf.Alcohol);
		}
	}
}