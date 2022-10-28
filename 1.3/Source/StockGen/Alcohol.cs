using RimWorld;
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
			return Things.Util.IsAlcohol(def);
		}

		private static readonly SimpleCurve SelectionWeight = new SimpleCurve
		{
			new CurvePoint(0f, 1f),
			new CurvePoint(15f, 0.6f),
			new CurvePoint(25f, 0.3f),
			new CurvePoint(400f, 0.1f),
			new CurvePoint(600f, 0.02f),
			new CurvePoint(1000f, 0.01f)
		};

		protected override float Weight(in ThingDef def, in int forTile, in Faction faction) => SelectionWeight.Evaluate(def.BaseMarketValue);
	}
}