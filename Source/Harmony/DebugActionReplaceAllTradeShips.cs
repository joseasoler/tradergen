using HarmonyLib;
using RimWorld;
using TG.Mod;
using Verse;

namespace TG.Harmony
{
	/// <summary>
	/// Patches DebugActionsMisc.ReplaceAllTradeShips to make the maximum number of ships configurable.
	/// </summary>
	public static class DebugActionReplaceAllShips
	{
		/// <summary>
		/// Applies Harmony patches required for this functionality.
		/// </summary>
		/// <param name="harmony">Harmony instance.</param>
		public static void Patch(HarmonyLib.Harmony harmony)
		{
			var replaceAllTradeShips = AccessTools.Method(typeof(DebugActionsMisc),
				nameof(DebugActionsMisc.ReplaceAllTradeShips));

			var replaceAllTradeShipsPrefix =
				new HarmonyMethod(AccessTools.Method(typeof(DebugActionReplaceAllShips), nameof(ReplaceAllTradeShipsPrefix)));
			harmony.Patch(replaceAllTradeShips, replaceAllTradeShipsPrefix);
		}

		/// <summary>
		/// Delegates orbital trade ship generation to OrbitalTraderArrival.
		/// </summary>
		/// <returns>False, preventing the original method from running.</returns>
		private static bool ReplaceAllTradeShipsPrefix()
		{
			Find.CurrentMap.passingShipManager.DebugSendAllShipsAway();

			for (var index = 0; index < Settings.MaxOrbitalShips; ++index)
			{
				IncidentDefOf.OrbitalTraderArrival.Worker.TryExecute(new IncidentParms
				{
					target = Find.CurrentMap
				});
			}

			return false;
		}
	}
}