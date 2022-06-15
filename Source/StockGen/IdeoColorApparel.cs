using System.Collections.Generic;
using RimWorld;
using Verse;

namespace TG.StockGen
{
	/// <summary>
	/// Generates apparel with the color of the trader's ideology.
	/// </summary>
	public abstract class IdeoColorApparel : ConditionMatcher
	{
		/// <summary>
		/// Defines which type of apparel this generator can use.
		/// </summary>
		/// <param name="def">Def is already known to be an apparel.</param>
		/// <returns>True if it can be sold/purchased.</returns>
		protected abstract bool ApparelCondition(in ThingDef def);

		protected override bool CanBuy(in ThingDef def)
		{
			return def.IsApparel && ApparelCondition(def);
		}

		protected override IEnumerable<Thing> TryMakeForStock(ThingDef def, Faction faction)
		{
			if (!ModsConfig.IdeologyActive || faction?.ideos?.PrimaryIdeo == null)
			{
				Logger.ErrorOnce("PreferredApparel should only be used when Ideology is active with trader having ideos.");
				yield break;
			}

			var color = faction.ideos.PrimaryIdeo.ApparelColor;
			foreach (var thing in base.TryMakeForStock(def, faction))
			{
				thing.SetColor(color);
				yield return thing;
			}
		}
	}
}