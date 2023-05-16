using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld.Planet;
using Verse;

namespace TraderGen.Harmony.Settlements
{
	/// It is only possible to know which seed to use after Settlement_TraderTracker.RegenerateStock has been called.
	/// But certain entities such as the trade dialog may want to know what the settlement can sell before the stock
	/// has been generated. Since it is not possible to generate the TraderGen information at that point, TraderGen
	/// forces the generation of stock to happen earlier.
	[HarmonyPatch]
	internal class CaravanVisitUtility_TradeCommand_Patch
	{
		static MethodBase TargetMethod()
		{
			var parentType = typeof(CaravanVisitUtility);
			foreach (var nested in parentType.GetNestedTypes(AccessTools.all))
			{
				if (nested.Name == "<>c__DisplayClass2_0")
				{
					return AccessTools.GetDeclaredMethods(nested)[0];
				}
			}

			return null;
		}

		private static void RegenerateStockIfRequired(Settlement settlement)
		{
			var trader = settlement.trader;
			if (trader.stock == null)
			{
			}
		}

		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo regenerateIfRequired =
				AccessTools.Method(typeof(SettlementPatchUtil), nameof(SettlementPatchUtil.RegenerateStockIfRequired));
			MethodInfo findWindowStack =
				AccessTools.PropertyGetter(typeof(Find), nameof(Find.WindowStack));

			foreach (var code in instructions)
			{
				// Regenerate stock right before the window is opened.
				if (code.opcode == OpCodes.Call && code.operand as MethodInfo == findWindowStack)
				{
					yield return new CodeInstruction(OpCodes.Ldloc_0); // Load settlement.
					yield return new CodeInstruction(OpCodes.Callvirt, regenerateIfRequired);
				}

				yield return code;
			}
		}
	}
}