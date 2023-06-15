using HarmonyLib;
using RimWorld;
using TraderGen.TraderKind;
using Verse;

namespace TraderGen.Harmony
{
	/// <summary>
	/// Patches required for trader pawns (visitors and caravans).
	/// </summary>
	[HarmonyPatch]
	public static class PawnGen
	{
		/// <summary>
		/// Sets the seed that may be used for random generation of visitor or caravan trader information.
		/// </summary>
		/// <param name="pawn">Pawn being updated.</param>
		/// <param name="actAsIfSpawned">Unused.</param>
		[HarmonyPostfix]
		[HarmonyPatch(typeof(PawnComponentsUtility), nameof(PawnComponentsUtility.AddAndRemoveDynamicComponents))]
		private static void PawnGeneration(Pawn pawn, bool actAsIfSpawned)
		{
			if (pawn?.trader != null)
			{
				Cache.SetSeed(pawn);
			}
		}

		/// <summary>
		/// Update TraderKind.Cache when a pawn with a trader tracker is spawning after load.
		/// It must be done after spawning because that is when the map of the pawn will have been set.
		/// </summary>
		/// <param name="__instance">Pawn instance</param>
		/// <param name="map">Map of the pawn.</param>
		/// <param name="respawningAfterLoad">True if the pawn is respawning after load.</param>
		[HarmonyPostfix]
		[HarmonyPatch(typeof(Pawn), nameof(Pawn.SpawnSetup))]
		private static void PawnLoad(Pawn __instance, Map map, bool respawningAfterLoad)
		{
			if (!respawningAfterLoad || __instance.trader?.traderKind == null)
			{
				return;
			}

			Cache.SetSeed(__instance);
			Cache.TryAdd(__instance.trader.traderKind, map?.Tile ?? -1, __instance.Faction);
		}

		/// <summary>
		/// Remove trader information of the pawn from the cache.
		/// </summary>
		/// <param name="__instance">Pawn instance</param>
		[HarmonyPrefix]
		[HarmonyPatch(typeof(Pawn), nameof(Pawn.DeSpawn))]
		private static void PawnClear(Pawn __instance)
		{
			if (__instance.trader == null) return;
			Cache.Remove(__instance);
		}
	}
}