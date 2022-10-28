using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using TG.TraderKind;
using Verse;

namespace TG.Harmony
{
	/// <summary>
	/// Patches the trade dialog to take into account TraderGen additions.
	/// </summary>
	public static class DialogTrade
	{
		/// <summary>
		/// Apply the Harmony patches for trade dialog compatibility.
		/// </summary>
		/// <param name="harmony">Harmony library instance.</param>
		public static void Patch(HarmonyLib.Harmony harmony)
		{
			if (HarmonyUtils.TradeUIRevisedActive()) return;

			var tradeWindowContents = AccessTools.Method(typeof(Dialog_Trade), nameof(Dialog_Trade.DoWindowContents));
			var tradeWindowTranspiler =
				new HarmonyMethod(
					AccessTools.Method(typeof(HarmonyUtils), nameof(HarmonyUtils.AddSpecializationsToTraderLabel)));
			harmony.Patch(tradeWindowContents, transpiler: tradeWindowTranspiler);
		}
	}
}