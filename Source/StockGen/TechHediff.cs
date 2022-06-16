using System;
using RimWorld;
using Verse;

namespace TG.StockGen
{
	/// <summary>
	/// Produces bionics with a selection curve that makes very cheap and very expensive bionics less common.
	/// </summary>
	public class TechHediff : ConditionMatcher
	{
		protected override bool ValidTechLevel(in ThingDef def)
		{
			return base.ValidTechLevel(def);
		}

		protected override bool CanBuy(in ThingDef def)
		{
			return def.tradeTags != null && def.tradeTags.Contains("TechHediff");
		}

		private static readonly SimpleCurve SelectionWeight = new SimpleCurve
		{
			new CurvePoint(50.0f, 0.3f),
			new CurvePoint(400.0f, 0.6f),
			new CurvePoint(800.0f, 1.0f),
			new CurvePoint(1000.0f, 0.8f),
			new CurvePoint(1300f, 0.6f),
			new CurvePoint(1600f, 0.3f),
			new CurvePoint(2000f, 0.1f)
		};

		protected override float Weight(in ThingDef def, in int forTile, in Faction faction) =>
			SelectionWeight.Evaluate(def.BaseMarketValue);
	}
}