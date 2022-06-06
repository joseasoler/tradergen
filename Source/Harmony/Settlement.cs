using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using TG.TraderKind;
using Verse;

namespace TG.Harmony
{
	/// <summary>
	/// Sets the seed that will be used for random generation of settlement trader information.
	/// </summary>
	[HarmonyPatch]
	public class SettlementGen
	{
		/// <summary>
		/// It is only possible to know which seed to use after Settlement_TraderTracker.RegenerateStock has been called.
		/// But certain entities such as the trade dialog may want to know what the settlement can sell before the stock
		/// has been generated. Since it is not possible to generate the TraderGen information at that point, TraderGen
		/// forces the generation of stock to happen earlier, during the "can this settlement trade" checks.
		/// </summary>
		/// <param name="__instance">Trader tracker instance</param>
		[HarmonyPrefix]
		[HarmonyPatch(typeof(Settlement_TraderTracker), nameof(Settlement_TraderTracker.CanTradeNow), MethodType.Getter)]
		private static void EnsureStockIsGenerated(Settlement_TraderTracker __instance)
		{
			if (__instance.stock != null || __instance.TraderKind == null) return;
			__instance.RegenerateStock();
		}

		/// <summary>
		/// Remove previous trader information of the settlement before a new one is generated, and add the new one.
		/// </summary>
		/// <param name="__instance">Settlement trader tracker instance</param>
		[HarmonyPrefix]
		[HarmonyPatch(typeof(Settlement_TraderTracker), nameof(Settlement_TraderTracker.RegenerateStock))]
		private static void SettlementRemoveAndGenerate(Settlement_TraderTracker __instance)
		{
			if (__instance.TraderKind == null) return;
			Cache.Remove(__instance.settlement);
			// Update lastStockGenerationTicks earlier than vanilla so Settlement will return the new seed now.
			__instance.lastStockGenerationTicks = Find.TickManager.TicksGame;
			Cache.SetSeed(__instance.settlement);
			Cache.TryAdd(__instance.TraderKind, __instance.settlement.Map?.Tile ?? -1, __instance.settlement.Faction);
		}

		/// <summary>
		/// Remove trader information of the settlement from the cache when the settlement is removed from the game.
		/// </summary>
		/// <param name="__instance">Settlement instance</param>
		[HarmonyPrefix]
		[HarmonyPatch(typeof(Settlement), nameof(Settlement.PostRemove))]
		private static void SettlementRemove(Settlement __instance)
		{
			Cache.Remove(__instance);
		}
	}
}