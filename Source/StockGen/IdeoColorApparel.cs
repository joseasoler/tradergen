using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace TraderGen.StockGen
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

		protected override IEnumerable<Thing> TryMakeForStock(ThingDef thingDef, Faction faction)
		{
			if (!ModsConfig.IdeologyActive )
			{
				Logger.ErrorOnce("PreferredApparel should only be used when Ideology is active.");
				yield break;
			}

			if (faction?.ideos?.PrimaryIdeo == null)
			{
				Logger.ErrorOnce("PreferredApparel can only be used by traders that have a faction and an idelogy.");
				yield break;
			}

			Color color = faction.ideos.PrimaryIdeo.ApparelColor;
			foreach (Thing thing in base.TryMakeForStock(thingDef, faction))
			{
				thing.SetColor(color);
				yield return thing;
			}
		}
	}
}