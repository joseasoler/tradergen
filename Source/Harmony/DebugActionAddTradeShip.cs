using System.Linq;
using HarmonyLib;
using RimWorld;
using TG.Trader;
using Verse;

namespace TG.Harmony
{
	/// <summary>
	/// Patches AddTradeShipOfKind to use TraderGenKind instances.
	/// This is not compatible with the Trader Ships mod. In this case it will generate a random ship.
	/// </summary>
	public static class DebugActionAddTradeShip
	{
		/// <summary>
		/// Applies Harmony patches required for this functionality.
		/// </summary>
		/// <param name="harmony">Harmony instance.</param>
		public static void Patch(HarmonyLib.Harmony harmony)
		{
			// Patching RimWorld.TradeShip.Depart is incompatible with Trader Ships. See TG.Harmony.Mod.TraderShips.
			if (HarmonyInitialization.TraderShipsModEnabled)
			{
				return;
			}

			var addTradeShip = AccessTools.Method(typeof(DebugActionsMisc), nameof(DebugActionsMisc.AddTradeShipOfKind));

			var prefixName = !HarmonyInitialization.TraderShipsModEnabled
				? nameof(AddTradeShipPrefix)
				: nameof(AddTradeShipModdedPrefix);
			var addTradeShipPrefix = new HarmonyMethod(typeof(DebugActionAddTradeShip), prefixName);

			harmony.Patch(addTradeShip, addTradeShipPrefix);
		}

		/// <summary>
		/// Shows a list of all TraderGenDefs. Clicking on one of them generates a new random orbital using it as template.
		/// </summary>
		/// <returns>False, preventing the original method from running.</returns>
		private static bool AddTradeShipPrefix()
		{
			var options = DefDatabase<TraderGenDef>.AllDefs.Where(t => t.orbital)
				.Select(genDef => new DebugMenuOption(genDef.label, DebugMenuOptionMode.Action, () =>
				{
					Find.CurrentMap.passingShipManager.DebugSendAllShipsAway();
					OrbitalTraderArrival.Arrive(new IncidentParms {target = Find.CurrentMap}, genDef);
				}))
				.ToList();
			Find.WindowStack.Add(new Dialog_DebugOptionListLister(options));
			return false;
		}

		/// <summary>
		/// Generates a new random orbital using a random TraderGenDef.
		/// </summary>
		/// <returns>False, preventing the original method from running.</returns>
		private static bool AddTradeShipModdedPrefix()
		{
			Find.CurrentMap.passingShipManager.DebugSendAllShipsAway();
			IncidentDefOf.OrbitalTraderArrival.Worker.TryExecute(new IncidentParms {target = Find.CurrentMap});
			return false;
		}
	}
}