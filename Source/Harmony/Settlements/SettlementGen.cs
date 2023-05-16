using HarmonyLib;
using RimWorld.Planet;
using TraderGen.TraderKind;
using Verse;

namespace TraderGen.Harmony.Settlements
{
	/// <summary>
	/// Sets the seed that will be used for random generation of settlement trader information.
	/// </summary>
	[HarmonyPatch]
	public static class SettlementGen
	{
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