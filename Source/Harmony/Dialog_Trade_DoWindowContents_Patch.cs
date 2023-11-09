using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using RimWorld;

namespace TraderGen.Harmony
{
	/// <summary>
	/// Patches the trade dialog to take into account TraderGen additions.
	/// </summary>
	[HarmonyPatch(typeof(Dialog_Trade), nameof(Dialog_Trade.DoWindowContents))]
	internal static class Dialog_Trade_DoWindowContents_Patch
	{
		internal static bool Prepare(MethodBase original)
		{
			return !HarmonyUtils.TradeUIRevisedActive();
		}

		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			return HarmonyUtils.AddSpecializationsToTraderLabel(instructions);
		}
	}
}