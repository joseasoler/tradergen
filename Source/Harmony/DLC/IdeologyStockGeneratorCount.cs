using HarmonyLib;
using RimWorld;
using TG.Ideo;
using Verse;

namespace TG.Harmony.DLC
{
	public static class StockGeneratorCount
	{
		/// <summary>
		/// Applies Harmony patches for StockGenerator count when Ideology is used.
		/// </summary>
		/// <param name="harmony">Harmony library instance.</param>
		public static void Patch(HarmonyLib.Harmony harmony)
		{
			if (!ModsConfig.IdeologyActive)
			{
				return;
			}

			var randomCountOf = AccessTools.Method(typeof(StockGenerator), nameof(StockGenerator.RandomCountOf));
			var randomCountOfPrefix = new HarmonyMethod(AccessTools.Method(typeof(StockGeneratorCount), nameof(ForbidStock)));
			harmony.Patch(randomCountOf, prefix: randomCountOfPrefix);
		}

		private static bool ForbidStock(ref StockGenerator __instance, ref int __result, ThingDef def)
		{
			if (IdeoStockCache.Instance.Stocks(__instance.trader.debugRandomId, def))
			{
				return true;
			}

			__result = 0;
			return false;
		}
	}
}