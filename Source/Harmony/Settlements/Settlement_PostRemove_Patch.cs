using HarmonyLib;
using RimWorld.Planet;
using TraderGen.TraderKind;

namespace TraderGen.Harmony.Settlements
{
	/// <summary>
	/// Remove trader information of the settlement from the cache when the settlement is removed from the game.
	/// </summary>
	[HarmonyPatch(typeof(Settlement), nameof(Settlement.PostRemove))]
	public static class Settlement_PostRemove_Patch
	{
		private static void Prefix(Settlement __instance)
		{
			Cache.Remove(__instance);
		}
	}
}