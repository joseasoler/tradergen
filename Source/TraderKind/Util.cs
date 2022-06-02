using RimWorld;
using Verse;

namespace TG.TraderKind
{
	/// <summary>
	/// Helper functions related to TraderKindDefs.
	/// </summary>
	public static class Util
	{
		/// <summary>
		/// Find out the factionDef of a trader.
		/// </summary>
		/// <param name="def">Trader being evaluate</param>
		/// <returns>FactionDef of the trader.</returns>
		public static FactionDef GetFactionDef(TraderKindDef def)
		{
			// The faction defined in the trader takes precedence.
			var traderFactionDef = def.faction;
			// If the trader has no faction, check if it is a base, caravan or visitor of a faction.
			if (traderFactionDef == null && !def.orbital)
			{
				foreach (var factionDef in DefDatabase<FactionDef>.AllDefs)
				{
					if (factionDef.baseTraderKinds.Contains(def) || factionDef.caravanTraderKinds.Contains(def) ||
					    factionDef.visitorTraderKinds.Contains(def))
					{
						traderFactionDef = factionDef;
						break;
					}
				}
			}

			// Return the found faction definition, if any.
			return traderFactionDef;
		}

		/// <summary>
		/// Find out the trader kind category of a trader.
		/// </summary>
		/// <param name="def">Trader being evaluated.</param>
		/// <param name="faction">Faction of the trader.</param>
		/// <returns>Trader category.</returns>
		public static TraderKindCategory GetTraderCategory(TraderKindDef def, Faction faction)
		{
			if (def.orbital)
			{
				return TraderKindCategory.Orbital;
			}

			if (faction.def.baseTraderKinds.Contains(def)) return TraderKindCategory.Settlement;
			if (faction.def.caravanTraderKinds.Contains(def)) return TraderKindCategory.Caravan;
			return faction.def.visitorTraderKinds.Contains(def) ? TraderKindCategory.Visitor : TraderKindCategory.None;
		}
	}
}