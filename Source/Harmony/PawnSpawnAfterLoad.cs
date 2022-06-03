using HarmonyLib;
using RimWorld;
using TG.TraderKind;
using Verse;

namespace TG.Harmony
{
	/// <summary>
	/// Patches required for loading pawns with a trader tracker.
	/// </summary>
	[HarmonyPatch]
	public class PawnLoad
	{
		/// <summary>
		/// Regenerate the TraderKindDef when a pawn with a trader tracker is spawning after load.
		/// It must be done after spawning because that is when the map of the pawn will have been set.
		/// Since the generated TraderKindDef has the same defName as the real one, TradeShip.ExposeData will "save"
		/// the real TraderKindDef. When it is loaded, it will also get the real TraderKindDef. After the loading process is
		/// finished, it is replaced with the generated TraderKindDef.
		/// </summary>
		/// <param name="__instance">Pawn_TraderTracker instance.</param>
		[HarmonyPostfix]
		[HarmonyPatch(typeof(Pawn), nameof(Pawn.SpawnSetup))]
		private static void LoadTraderKindDef(ref Pawn __instance, Map map, bool respawningAfterLoad)
		{
			if (respawningAfterLoad && __instance.trader != null)
			{
				__instance.trader.traderKind =
					Generator.Def(__instance.trader.traderKind, __instance.RandomPriceFactorSeed, __instance);
			}
		}
	}
}