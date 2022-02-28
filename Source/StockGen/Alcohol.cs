using System.Linq;
using RimWorld;
using TG.DefOf;
using Verse;

namespace TG.StockGen
{
	/// <summary>
	/// Handles alcoholic beverages, but excludes non-alcoholic beverages and alcoholic foodstuffs like rotting mounds.
	/// Expensive liquors such as high-quality ambrandy from Vanilla Brewing Expanded are very infrequent.
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

		private static readonly SimpleCurve SelectionWeight = new SimpleCurve
		{
			new CurvePoint(0.0f, 1f),
			new CurvePoint(400f, 0.5f),
			new CurvePoint(800f, 0.2f),
			new CurvePoint(2000f, 0.1f)
		};

		protected override float Weight(in ThingDef def, in int forTile, in Faction faction) => SelectionWeight.Evaluate(def.BaseMarketValue);
	}
}