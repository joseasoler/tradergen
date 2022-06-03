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
	public class PawnTraderTracker
	{
		/// <summary>
		/// Regenerate the TraderKindDef after loading a pawn tracker.
		/// Since the generated TraderKindDef has the same defName as the real one, TradeShip.ExposeData will "save"
		/// the real TraderKindDef. When it is loaded, it will also get the real TraderKindDef. After the loading process is
		/// finished, it is replaced with the generated TraderKindDef.
		/// </summary>
		/// <param name="__instance">Pawn_TraderTracker instance.</param>
		[HarmonyPostfix]
		[HarmonyPatch(typeof(Pawn_TraderTracker), nameof(Pawn_TraderTracker.ExposeData))]
		private static void LoadTraderKindDef(ref Pawn_TraderTracker __instance)
		{
			// Wait until the pawn tracker is fully loaded. 
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				__instance.traderKind = Generator.Def(__instance.traderKind, __instance.RandomPriceFactorSeed,
					__instance.pawn.Map, __instance.pawn.Faction);
			}
		}
	}
}