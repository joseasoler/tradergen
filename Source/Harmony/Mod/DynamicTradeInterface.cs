using System.Reflection;
using HarmonyLib;
using RimWorld;
using TraderGen.TraderKind;

namespace TraderGen.Harmony.Mod
{
	[HarmonyPatch]
	internal static class DynamicTradeInterface
	{
		internal static bool Prepare(MethodBase original)
		{
			
			return ModAssemblies.DynamicTradeInterface() != null;
		}

		internal static MethodBase TargetMethod()
		{
			var method = ModAssemblies.GetMethod(ModAssemblies.DynamicTradeInterface(), "Window_DynamicTrade", "PreOpen");
			return method;
		}

		[HarmonyPostfix]
		internal static void ReplaceTraderHeaderDescription(ref string ____traderHeaderDescription)
		{
			____traderHeaderDescription = Util.Label(TradeSession.trader);
		}
	}
}