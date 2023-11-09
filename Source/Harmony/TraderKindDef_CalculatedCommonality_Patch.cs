using HarmonyLib;
using RimWorld;
using TG.Mod;

namespace TraderGen.Harmony
{
	/// <summary>
	/// Overrides commonalityMultFromPopulationIntent if requested.
	/// </summary>
	[HarmonyPatch(typeof(TraderKindDef), nameof(TraderKindDef.CalculatedCommonality), MethodType.Getter)]
	public static class TraderKindDef_CalculatedCommonality_Patch
	{
		private static void Postfix(ref TraderKindDef __instance, ref float __result)
		{
			if (Settings.IgnoreColonyPopulationCommonality && __instance.commonalityMultFromPopulationIntent != null)
			{
				__result = __instance.commonality;
			}
		}
	}
}