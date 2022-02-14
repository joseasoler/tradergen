using System;
using System.Linq;
using HarmonyLib;
using RimWorld;
using TG.Trader;
using Verse;

namespace TG.Harmony
{
	/// <summary>
	/// Overrides IncidentWorker_OrbitalTraderArrival to hand generation over to ProceduralTradeShip.
	/// </summary>
	[HarmonyPatch(typeof(IncidentWorker_OrbitalTraderArrival), "TryExecuteWorker")]
	public class IncidentWorker_OrbitalTraderArrival_TryExecuteWorker
	{
		/// <summary>
		/// Hands over the TraderKindDef generation to ProceduralTradeShipArrival.
		/// </summary>
		/// <param name="__result">True if an orbital trader was generated successfully.</param>
		/// <param name="parms">Incident generation parameters.</param>
		/// <returns>Always false, as this prefix takes over vanilla orbital trader generation.</returns>
		[HarmonyPrefix]
		static bool Prefix(ref bool __result, IncidentParms parms)
		{
			__result = ProceduralTradeShipArrival.TryArrival(parms);
			return false;
		}
	}
}