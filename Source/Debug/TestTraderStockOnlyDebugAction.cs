using System.Collections.Generic;
using System.Linq;
using Force.DeepCloner;
using RimWorld;
using TraderGen.TraderKind;
using Verse;

namespace TraderGen.Debug
{
	/// <summary>
	/// Adds an orbital trader using the stock of any TraderKindDef even if they are not orbital.
	/// </summary>
	public static class TestTraderStockOnlyDebugAction
	{
		private static void GenerateTraderAsOrbital(TraderKindDef traderKindDef)
		{
			// Avoid modifying real game defs. Don't use this in a save-game you intend to keep!
			TraderKindDef newDef = traderKindDef.ShallowClone();

			// IncidentWorker_OrbitalTraderArrival uses this to set the faction. IncidentParms.faction is ignored.
			// But IncidentParms.faction is used in other cases such as when generating stock.
			newDef.faction = Util.GetFactionDef(traderKindDef);
			newDef.orbital = true;
			if (string.IsNullOrEmpty(newDef.label))
			{
				newDef.label = newDef.defName;
			}

			Find.CurrentMap.passingShipManager.DebugSendAllShipsAway();
			Faction factionParameter = newDef.faction == null
				? null
				: Find.FactionManager.AllFactions.FirstOrDefault(f => f.def == newDef.faction);
			IncidentDefOf.OrbitalTraderArrival.Worker.TryExecute(new IncidentParms
				{ target = Find.CurrentMap, traderKind = newDef, faction = factionParameter });
		}

		[DebugAction(DebugActions.DebugCategory, null, false, false, allowedGameStates = AllowedGameStates.PlayingOnMap)]
		public static void TestTraderStockOnly()
		{
			List<DebugMenuOption> menuOptions = new List<DebugMenuOption>();
			List<TraderKindDef> traderKindDefs = DefDatabase<TraderKindDef>.AllDefsListForReading;
			for (int traderKindDefIndex = 0; traderKindDefIndex < traderKindDefs.Count; ++traderKindDefIndex)
			{
				TraderKindDef traderKindDef = traderKindDefs[traderKindDefIndex];
				menuOptions.Add(new DebugMenuOption(traderKindDef.defName, DebugMenuOptionMode.Action,
					() => GenerateTraderAsOrbital(traderKindDef)));
			}

			menuOptions.Sort(DebugActions.CompareDebugMenuOptions);
			Find.WindowStack.Add(new Dialog_DebugOptionListLister(menuOptions));
		}
	}
}