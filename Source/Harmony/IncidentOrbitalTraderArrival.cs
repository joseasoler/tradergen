using HarmonyLib;
using RimWorld;
using TG.Trader;

namespace TG.Harmony
{
	/// <summary>
	/// Patches IncidentWorker_OrbitalTraderArrival to make the maximum number of ships configurable.
	/// Incompatible and unneeded with the Trader Ships mod.
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
		/// Delegates orbital trade ship generation to OrbitalTraderArrival.
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