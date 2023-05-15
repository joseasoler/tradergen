using RimWorld;
using Verse;

namespace TraderGen.StockGen
{
	/// <summary>
	/// Reimplementation of StockGenerator_Art. Required in order to apply forceFactionTechLimits.
	/// </summary>
	public class Art : ConditionMatcher
	{
		protected override bool CanBuy(in ThingDef def)
		{
			return def.Minifiable && def.category == ThingCategory.Building && def.thingClass == typeof(Building_Art);
		}

		protected override float Weight(in ThingDef def, in int forTile, in Faction faction)
		{
			return StockGenerator_MarketValue.SelectionWeightMarketValueCurve.Evaluate(def.BaseMarketValue);
		}
	}
}