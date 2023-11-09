using System.Reflection;
using HarmonyLib;
using RimWorld;
using TraderGen.TraderKind;

namespace TraderGen.Harmony.Mod
{
	/// <summary>
	/// Include trader specializations in the trade window from Dynamic Trade Interface.
	/// </summary>
	[HarmonyPatch]
	internal static class DynamicTradeInterface_Window_DynamicTrade_PreOpen_Patch
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
			____traderHeaderDescription = Util.LabelWithTraderSpecialization(TradeSession.trader);
		}
	}
}