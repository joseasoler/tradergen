using System.Linq;
using System.Reflection;
using HarmonyLib;
using Verse;

namespace TraderGen.Harmony.Mod
{
	public static class TradeUIRevised
	{
		/// <summary>
		/// Apply the Harmony patch for Trade UI Revised compatibility only if the mod is loaded.
		/// </summary>
		/// <param name="harmony">Harmony library instance.</param>
		public static void Patch(HarmonyLib.Harmony harmony)
		{
			if (!HarmonyUtils.TradeUIRevisedActive()) return;

			var doWindowContents = AccessTools.Method("TradeUI.TradeUIRework+Harmony_DialogTrade_FillMainRect:Prefix");
			var tradeWindowTranspiler =
				new HarmonyMethod(
					AccessTools.Method(typeof(HarmonyUtils), nameof(HarmonyUtils.AddSpecializationsToTraderLabel)));
			harmony.Patch(doWindowContents, transpiler: tradeWindowTranspiler);
		}
	}
}