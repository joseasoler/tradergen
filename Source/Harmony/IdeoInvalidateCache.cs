using HarmonyLib;
using RimWorld;
using TG.Ideo;
using TG.Mod;
using Verse;

namespace TG.Harmony
{
	[HarmonyPatch]
	public static class IdeoInvalidateCache
	{
		/// <summary>
		/// After precepts have been cached, invalidate the IdeoStockCache.
		/// </summary>
		[HarmonyPostfix]
		[HarmonyPatch(typeof(RimWorld.Ideo), nameof(RimWorld.Ideo.RecachePrecepts))]
		private static void IdeoInvalidateCacheAfterPreceptsUpdate(ref RimWorld.Ideo __instance)
		{
			IdeoStockCache.Instance.Invalidate(__instance);
		}
	}
}