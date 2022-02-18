using System;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using TG.Gen;
using Verse;

namespace TG.Harmony.Mod
{
	/// <summary>
	/// The Trader Ships mod is not compatible with the Depart Harmony patch that TraderGen usually uses.
	/// This class patches all Trader Ships methods responsible for TradeShip instances being destroyed to ensure the
	/// removal of their procedurally generated TraderKindDefs.
	/// IncidentWorkerTraderShip is also patched to push and, later, pop a new seed into Rand.
	/// This forces all random generation related to the trader (including ship sprites) to use the same seed.
	/// </summary>
	public class TraderShips
	{
		/// <summary>
		/// Trade ship class field of the TraderShips.CompShip class.
		/// </summary>
		private static FieldInfo _tradeShipField;

		/// <summary>
		/// Applies Harmony patches required for this mod.
		/// </summary>
		/// <param name="harmony">Harmony instance.</param>
		public static void Patch(HarmonyLib.Harmony harmony)
		{
			if (!HarmonyInitialization.TraderShipsModEnabled)
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

			var tryExecuteWorkerPub = AccessTools.Method("TraderShips.IncidentWorkerTraderShip:TryExecuteWorkerPub");
			var tryExecuteWorkerPubPrefix =
				new HarmonyMethod(AccessTools.Method(typeof(TraderShips), nameof(TryExecuteWorkerPubPrefix)));
			var tryExecuteWorkerPubPostfix =
				new HarmonyMethod(AccessTools.Method(typeof(TraderShips), nameof(TryExecuteWorkerPubPostfix)));
			harmony.Patch(tryExecuteWorkerPub, tryExecuteWorkerPubPrefix, tryExecuteWorkerPubPostfix);
		}

		/// <summary>
		/// Obtain the TraderKindDef of a TraderShips.CompShip instance.
		/// </summary>
		/// <param name="instance">Assumed to be a TraderShips.CompShip instance.</param>
		/// <returns>TraderKindDef used by the ship.</returns>
		private static TraderKindDef GetDef(in ThingComp instance)
		{
			return ((RimWorld.TradeShip) _tradeShipField.GetValue(instance)).def;
		}

		/// <summary>
		/// Clean up the generated TraderKindDef just before the ship departs.
		/// </summary>
		/// <param name="__instance">Departing TraderShips.CompShip instance</param>
		/// <returns>True, allowing the original method to run.</returns>
		private static bool SendAwayPrefix(in ThingComp __instance)
		{
			Find.World.GetComponent<TraderKind>().Remove(GetDef(__instance));
			// Carry on with the execution of the original method.
			return true;
		}

		/// <summary>
		/// Clean up the generated TraderKindDef after the ship crashes and its contents have already been generated.
		/// </summary>
		/// <param name="__instance">Crashing TraderShips.CompShip instance</param>
		private static void CrashPostfix(in ThingComp __instance)
		{
			Find.World.GetComponent<TraderKind>().Remove(GetDef(__instance));
		}

		/// <summary>
		/// Push a new random seed into Rand.
		/// </summary>
		/// <returns>True, allowing the original method to run.</returns>
		private static bool TryExecuteWorkerPubPrefix()
		{
			Rand.PushState(Math.Abs(Rand.Int));
			// Carry on with the execution of the original method.
			return true;
		}
		
		/// <summary>
		/// Pop the seed previously added into Rand.
		/// </summary>
		private static void TryExecuteWorkerPubPostfix()
		{
			Rand.PopState();
		}
	}
}