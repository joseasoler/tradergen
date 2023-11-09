using HarmonyLib;
using TraderGen.TraderKind;
using Verse;

namespace TraderGen.Harmony.TraderPawns
{
	/// <summary>
	/// Update TraderKind.Cache when a pawn with a trader tracker is spawning after load.
	/// It must be done after spawning because that is when the map of the pawn will have been set.
	/// </summary>
	[HarmonyPatch(typeof(Pawn), nameof(Pawn.SpawnSetup))]
	internal static class Pawn_SpawnSetup_Patch
	{
		private static void Postfix(Pawn __instance, Map map, bool respawningAfterLoad)
		{
			
			if (!respawningAfterLoad || __instance.trader?.traderKind == null)
			{
				return;
			}

			Cache.SetSeed(__instance);
			Cache.TryAdd(__instance.trader.traderKind, map?.Tile ?? -1, __instance.Faction);
		}
	}
}