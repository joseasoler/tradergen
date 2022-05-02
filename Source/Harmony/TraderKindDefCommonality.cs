using HarmonyLib;
using RimWorld;
using TG.Mod;

namespace TG.Harmony
{
	[HarmonyPatch]
	public static class TraderKindDefCommonality
	{
		/// <summary>
		/// Overrides commonalityMultFromPopulationIntent if requested.
		/// </summary>
		[HarmonyPostfix]
		[HarmonyPatch(typeof(TraderKindDef), nameof(TraderKindDef.CalculatedCommonality), MethodType.Getter)]
		private static void CalculatedCommonalityPostfix(ref TraderKindDef __instance, ref float __result)
		{
			if (Settings.IgnoreColonyPopulationCommonality && __instance.commonalityMultFromPopulationIntent != null)
			{
				__result = __instance.commonality;
			}
		}
	}
}