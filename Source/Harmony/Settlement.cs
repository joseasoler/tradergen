using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using TG.Settlement;
using Verse;

namespace TG.Harmony
{
	[HarmonyPatch]
	public class Settlement
	{
		[HarmonyPrefix]
		[HarmonyPatch(typeof(Settlement_TraderTracker), nameof(TraderKind), MethodType.Getter)]
		private static bool GeneratedTraderKind(Settlement_TraderTracker __instance, ref TraderKindDef __result)
		{
			__result = Current.Game.World.GetComponent<SettlementTraderData>().Get(__instance.settlement);
			return false;
		}
	}
}