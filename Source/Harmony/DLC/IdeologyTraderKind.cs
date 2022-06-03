using HarmonyLib;
using RimWorld;
using TG.Ideo;
using Verse;
using Thing = TG.DefOf.Thing;

namespace TG.Harmony.DLC
{
	public static class IdeologyTraderKind
	{
		/// <summary>
		/// Apply the Harmony patches for TraderKinds when Ideology is used.
		/// </summary>
		/// <param name="harmony">Harmony library instance.</param>
		public static void Patch(HarmonyLib.Harmony harmony)
		{
			if (!ModsConfig.IdeologyActive)
			{
				return;
			}

			var willTrade = AccessTools.Method(typeof(TraderKindDef), nameof(TraderKindDef.WillTrade));
			var willTradePrefix = new HarmonyMethod(AccessTools.Method(typeof(IdeologyTraderKind), nameof(WillTradePrefix)));
			harmony.Patch(willTrade, prefix: willTradePrefix);
		}

		private static bool WillTradePrefix(ref TraderKindDef __instance, ref bool __result, ThingDef td)
		{
			var ideoId = (int) __instance.debugRandomId;
			__result = IdeoStockCache.Instance.Purchases(ideoId, td);

			// If the ideology is not willing to trade the item, there is no need to check the stockGenerators. 
			return __result;
		}
	}
}