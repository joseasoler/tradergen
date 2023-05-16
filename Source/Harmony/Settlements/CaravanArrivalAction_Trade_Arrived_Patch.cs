using HarmonyLib;
using RimWorld.Planet;

namespace TraderGen.Harmony.Settlements
{
	/// It is only possible to know which seed to use after Settlement_TraderTracker.RegenerateStock has been called.
	/// But certain entities such as the trade dialog may want to know what the settlement can sell before the stock
	/// has been generated. Since it is not possible to generate the TraderGen information at that point, TraderGen
	/// forces the generation of stock to happen earlier.
	[HarmonyPatch(typeof(CaravanArrivalAction_Trade), nameof(CaravanArrivalAction_Trade.Arrived))]
	internal static class CaravanArrivalAction_Trade_Arrived_Patch
	{
		private static void Prefix(CaravanArrivalAction_Trade __instance)
		{
			SettlementPatchUtil.RegenerateStockIfRequired(__instance.settlement);
		}
	}
}