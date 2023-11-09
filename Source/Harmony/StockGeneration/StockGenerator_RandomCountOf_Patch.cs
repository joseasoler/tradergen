using HarmonyLib;
using RimWorld;
using TraderGen.TraderKind;
using Verse;

namespace TraderGen.Harmony.StockGenerators
{
	/// <summary>
	/// Prevent stockGenerators from generating items forbidden by TraderGen.
	/// </summary>
	[HarmonyPatch(typeof(StockGenerator), nameof(StockGenerator.RandomCountOf))]
	internal static class StockGenerator_RandomCountOf_Patch
	{
		private static bool Prefix(ref int __result, ThingDef def)
		{
			if (Cache.WillNotStock(Cache.GenerationSeed, def))
			{
				__result = 0;
				return false;
			}

			return true;
		}
	}
}