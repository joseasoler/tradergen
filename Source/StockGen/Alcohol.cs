using System.Linq;
using RimWorld;
using TG.DefOf;
using Verse;

namespace TG.StockGen
{
	/// <summary>
	/// Handles alcoholic beverages, but excludes non-alcoholic beverages and alcoholic foodstuffs like rotting mounds.
	/// </summary>
	public class Alcohol : ConditionMatcher
	{
		protected override bool CanBuy(in ThingDef def)
		{
			if (def.ingestible?.outcomeDoers == null || def.ingestible.JoyKind != JoyKind.Chemical ||
			    !def.ingestible.foodType.HasFlag(FoodTypeFlags.Liquor)) return false;

			return (from outcomeDoer in def.ingestible.outcomeDoers
				where outcomeDoer.GetType() == typeof(IngestionOutcomeDoer_GiveHediff)
				select (IngestionOutcomeDoer_GiveHediff) outcomeDoer).Any(o =>
				o.hediffDef?.hediffClass == typeof(Hediff_Alcohol));
		}
	}
}