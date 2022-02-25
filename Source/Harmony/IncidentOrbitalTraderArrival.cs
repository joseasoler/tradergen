using System;
using HarmonyLib;
using RimWorld;
using TG.Trader;
using Verse;

namespace TG.Harmony
{
	/// <summary>
	/// Patches IncidentWorker_OrbitalTraderArrival to make the maximum number of ships configurable.
	/// Incompatible and unneeded with the Trader Ships mod.
	/// It sets a new seed into Rand, ensuring that all random generation related to the trader uses the same seed.
	/// This includes the generation of the stock itself. If in the future the same ship needs to be generated again,
	/// it may be necessary to push a new random seed for TradeShip.GenerateThings, and then pop it after it is done.
	/// </summary>
	public static class IncidentOrbitalTraderArrival
	{
		/// <summary>
		/// Applies Harmony patches required for this functionality.
		/// </summary>
		/// <param name="harmony">Harmony instance.</param>
		public static void Patch(HarmonyLib.Harmony harmony)
		{
			if (HarmonyInitialization.TraderShipsModEnabled)
			{
				return;
			}

			var tryExecuteWorker = AccessTools.Method(typeof(IncidentWorker_OrbitalTraderArrival),
				nameof(IncidentWorker_OrbitalTraderArrival.TryExecuteWorker));

			var tryExecuteWorkerPrefix =
				new HarmonyMethod(AccessTools.Method(typeof(IncidentOrbitalTraderArrival), nameof(TryExecuteWorkerPrefix)));
			harmony.Patch(tryExecuteWorker, tryExecuteWorkerPrefix);
		}

		/// <summary>
		/// Sets a random seed into Rand and delegates orbital trade ship generation to OrbitalTraderArrival.
		/// </summary>
		/// <param name="parms">Incident parameters.</param>
		/// <returns>False, preventing the original method from running.</returns>
		private static bool TryExecuteWorkerPrefix(in IncidentParms parms)
		{
			OrbitalTraderArrival.Arrive(parms, null);
			return false;
		}
	}
}