using HarmonyLib;
using RimWorld;
using TG.TraderKind;
using Verse;

namespace TG.Harmony
{
	/// <summary>
	/// Applies StockGenerator patches required for TraderGen features.
	/// </summary>
	[HarmonyPatch]
	public static class StockGeneratorGen
	{
		/// <summary>
		/// StockGenerators will not generate items forbidden by TraderGen.
		/// </summary>
		/// <param name="__result"></param>
		/// <param name="def"></param>
		/// <returns></returns>
		[HarmonyPrefix]
		[HarmonyPatch(typeof(StockGenerator), nameof(StockGenerator.RandomCountOf))]
		private static bool ForbidStock(ref int __result, ThingDef def)
		{
			if (!Cache.WillNotStock(Cache.GenerationSeed, def)) return true;
			__result = 0;
			return false;
		}
	}
}