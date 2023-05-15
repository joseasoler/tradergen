using RimWorld;
using Verse;

namespace TraderGen.StockGen
{
	/// <summary>
	/// Handles Jewelry from the Jewelry mod. Pieces under 250 silver are significantly more common.
	/// </summary>
	public class Jewelry : ConditionMatcher
	{
		protected override bool CanBuy(in ThingDef def)
		{
			return def.IsApparel && def.apparel.tags != null && def.apparel.tags.Contains("Jewelry");
		}

		private static readonly SimpleCurve SelectionWeight = new SimpleCurve
		{
			new CurvePoint(0.0f, 1f),
			new CurvePoint(250f, 0.5f),
			new CurvePoint(1000f, 0.2f),
			new CurvePoint(2000f, 0.1f)
		};

		protected override float Weight(in ThingDef def, in int forTile, in Faction faction) =>
			SelectionWeight.Evaluate(def.BaseMarketValue);
	}
}