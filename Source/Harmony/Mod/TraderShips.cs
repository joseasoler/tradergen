using System.Reflection;
using HarmonyLib;
using RimWorld;
using TG.Gen;
using Verse;

namespace TG.Harmony.Mod
{
	public class TraderShips
	{
		private static FieldInfo _tradeShipField;

		public static void Patch(HarmonyLib.Harmony harmony)
		{
			if (LoadedModManager.RunningMods.FirstIndexOf(pack => pack.PackageId.Equals("automatic.traderships")) < 0)
			{
				return;
			}

			// Initialize the FieldInfo used to access TraderKindDefs inside of CompShips.
			var compShipType = AccessTools.TypeByName("TraderShips.CompShip");
			_tradeShipField = compShipType.GetField("tradeShip", BindingFlags.Public | BindingFlags.Instance);

			var sendAway = AccessTools.Method("TraderShips.CompShip:SendAway");
			var sendAwayPrefix = new HarmonyMethod(AccessTools.Method(typeof(TraderShips), nameof(SendAwayPrefix)));
			harmony.Patch(sendAway, sendAwayPrefix);

			var crash = AccessTools.Method("TraderShips.CompShip:Crash");
			var crashPostfix = new HarmonyMethod(AccessTools.Method(typeof(TraderShips), nameof(CrashPostfix)));
			harmony.Patch(crash, postfix: crashPostfix);
		}

		/// <summary>
		/// Obtain the TraderKindDef of a TraderShips.CompShip instance.
		/// </summary>
		/// <param name="instance">Assumed to be a TraderShips.CompShip instance.</param>
		/// <returns>TraderKindDef used by the ship.</returns>
		private static TraderKindDef GetDef(ThingComp instance)
		{
			return ((RimWorld.TradeShip) _tradeShipField.GetValue(instance)).def;
		}

		/// <summary>
		/// Clean up the generated TraderKindDef when the ship departs.
		/// </summary>
		private static bool SendAwayPrefix(ThingComp __instance)
		{
			Find.World.GetComponent<TraderKind>().Remove(GetDef(__instance));
			// Carry on with the execution of the original method.
			return true;
		}

		/// <summary>
		/// Clean up the generated TraderKindDef after the ship crashes.
		/// </summary>
		private static void CrashPostfix(ThingComp __instance)
		{
			Find.World.GetComponent<TraderKind>().Remove(GetDef(__instance));
		}
	}
}