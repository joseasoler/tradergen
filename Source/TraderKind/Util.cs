using System.Linq;
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
		/// Find out the trader kind category of a trader.
		/// </summary>
		/// <param name="def">Trader being evaluate</param>
		/// <returns>Trader category.</returns>
		public static TraderKindCategory GetCategory(TraderKindDef def)
		{
			if (def.orbital)
			{
				return TraderKindCategory.Orbital;
			}

			foreach (var factionDef in DefDatabase<FactionDef>.AllDefs)
			{
				if (factionDef.baseTraderKinds.Contains(def)) return TraderKindCategory.Settlement;
				if (factionDef.caravanTraderKinds.Contains(def)) return TraderKindCategory.Caravan;
				if (factionDef.visitorTraderKinds.Contains(def)) return TraderKindCategory.Visitor;
			}

			return TraderKindCategory.None;
		}
	}
}