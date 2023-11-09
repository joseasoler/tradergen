using HarmonyLib;
using RimWorld.Planet;
using TraderGen.TraderKind;
using Verse;

namespace TraderGen.Harmony.Settlements
{
	/// <summary>
	/// Remove previous trader information of the settlement before a new one is generated, and add the new one.
	/// </summary>
	[HarmonyPatch(typeof(Settlement_TraderTracker), nameof(Settlement_TraderTracker.RegenerateStock))]
	public class Settlement_TraderTracker_RegenerateStock_Patch
	{
		private static void Prefix(Settlement_TraderTracker __instance)
		{
			if (__instance.TraderKind == null)
			{
				return;
			}

			Cache.Remove(__instance.settlement);
			// Update lastStockGenerationTicks earlier than vanilla so Settlement can return the new seed now.
			__instance.lastStockGenerationTicks = Find.TickManager.TicksGame;
			Cache.SetSeed(__instance.settlement);
			Cache.TryAdd(__instance.TraderKind, __instance.settlement.Map?.Tile ?? -1, __instance.settlement.Faction);
		}
	}
}