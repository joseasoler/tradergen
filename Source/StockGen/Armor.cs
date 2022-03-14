using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace TG.StockGen
{
	/// <summary>
	/// Matches any apparel which should be considered armor and matches certain trade tag conditions.
	/// </summary>
	public class Armor : ConditionMatcher
	{
		/// <summary>
		/// The armor must have one of these trade tags.
		/// </summary>
		public List<string> tradeTags;

		/// <summary>
		/// The armor must not have any of these trade tags.
		/// </summary>
		public List<string> excludeTradeTags;

		public override void ConditionToText(ref StringBuilder b)
		{
			base.ConditionToText(ref b);
			Util.ToText(ref b, "tradeTags", tradeTags);
			Util.ToText(ref b, "excludeTradeTags", excludeTradeTags);
		}

		protected override bool CanBuy(in ThingDef def)
		{
			if (tradeTags != null && (def.tradeTags == null || !def.tradeTags.Intersect(tradeTags).Any()))
			{
				return false;
			}

			if (excludeTradeTags != null && def.tradeTags != null && def.tradeTags.Intersect(excludeTradeTags).Any())
			{
				return false;
			}

			return Util.IsArmor(def);
		}

		/// <summary>
		/// Use the same weight calculation as StockGenerator_Armor.
		/// </summary>
		/// <param name="def">Thing being checked</param>
		/// <param name="forTile">Tile in which the transaction takes place.</param>
		/// <param name="faction">Faction of the trader.</param>
		/// <returns>Random weight of the def.</returns>
		protected override float Weight(in ThingDef def, in int forTile, in Faction faction)
		{
			return StockGenerator_Armor.SelectionWeightMarketValueCurve.Evaluate(def.BaseMarketValue);
		}
	}
}