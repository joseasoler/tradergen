using HarmonyLib;
using RimWorld;
using TG.TraderKind;

namespace TG.Harmony
{
	/// <summary>
	/// Patches Tradeable to implement TraderGen features.
	/// </summary>
	[HarmonyPatch]
	public class TradeableGen
	{
		/// <summary>
		/// WillTrade results are cached by TraderGen. They may also be overriden by certain features such as Ideology.
		/// </summary>
		/// <param name="__instance">Tradeable instance</param>
		/// <param name="__result">True if the trader is willing to trade this item.</param>
		/// <returns>Always false. TraderGen takes over the execution of this function.</returns>
		[HarmonyPrefix]
		[HarmonyPatch(typeof(Tradeable), nameof(Tradeable.TraderWillTrade), MethodType.Getter)]
		private static bool TradeAllowed(Tradeable __instance, ref bool __result)
		{
			__result = Cache.WillTrade(TradeSession.trader, TradeSession.trader.TraderKind, __instance.ThingDef);
			return false;
		}
	}
}