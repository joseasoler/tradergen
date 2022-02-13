using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace TG.Harmony
{
	[HarmonyPatch(typeof(DebugActionsMisc), "AddTradeShipOfKind")]
	public class DebugActionsMisc_AddTradeShipOfKind
	{
		[HarmonyPrefix]
		static bool Prefix()
		{
			var options = DefDatabase<TraderGenDef>.AllDefs.Where(t => t.orbital)
				.Select(genDef => new DebugMenuOption(genDef.label, DebugMenuOptionMode.Action, () =>
				{
					Find.CurrentMap.passingShipManager.DebugSendAllShipsAway();
					IncidentWorker_OrbitalTraderArrival_TryExecuteWorker.DoOrbitalTraderArrival(genDef, new IncidentParms {target = Find.CurrentMap});
				}))
				.ToList();
			Find.WindowStack.Add(new Dialog_DebugOptionListLister(options));
			return false;
		}
	}
}