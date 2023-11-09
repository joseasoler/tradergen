using HarmonyLib;
using RimWorld;
using TraderGen.TraderKind;
using Verse;

namespace TraderGen.Harmony.TraderPawns
{
	/// <summary>
	/// Sets the seed to use for random generation of trader information for visitors or caravans.
	/// </summary>
	[HarmonyPatch(typeof(PawnComponentsUtility), nameof(PawnComponentsUtility.AddAndRemoveDynamicComponents))]
	internal static class PawnComponentsUtility_AddAndRemoveDynamicComponents_Patch
	{
		private static void Postfix(Pawn pawn)
		{
			if (pawn?.trader != null)
			{
				Cache.SetSeed(pawn);
			}
		}
	}
}