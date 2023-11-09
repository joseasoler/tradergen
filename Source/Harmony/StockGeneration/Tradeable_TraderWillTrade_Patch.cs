using HarmonyLib;
using RimWorld;
using TraderGen.TraderKind;

namespace TraderGen.Harmony.StockGeneration
{
	/// <summary>
	/// WillTrade results are cached by TraderGen. They may also be overriden by certain features such as Ideology.
	/// </summary>
	[HarmonyPatch(typeof(Tradeable), nameof(Tradeable.TraderWillTrade), MethodType.Getter)]
	internal static class Tradeable_TraderWillTrade_Patch
	{
		private static bool Prefix(Tradeable __instance, ref bool __result)
		{
			__result = Cache.WillTrade(TradeSession.trader, TradeSession.trader.TraderKind, __instance.ThingDef);
			return false;
		}
	}
}