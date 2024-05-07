using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using TraderGen.TraderKind;
using Verse;

namespace TraderGen.Harmony
{
	/// <summary>
	/// Use the TraderGen cache to determine tradeability.
	/// </summary>
	[HarmonyPatch(typeof(Dialog_SellableItems), nameof(Dialog_SellableItems.CalculateSellableItems))]
	internal static class Dialog_SellableItems_CalculateSellableItems_Patch
	{
		public static bool WillTrade(TraderKindDef traderKindDef, ThingDef thingDef, ITrader trader)
		{
			return Cache.WillTrade(trader, traderKindDef, thingDef);
		}

		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo vanillaWillTradeMethod =
				AccessTools.Method(typeof(TraderKindDef),
					nameof(TraderKindDef.WillTrade));

			MethodInfo modifiedWillTradeMethod =
				AccessTools.Method(typeof(Dialog_SellableItems_CalculateSellableItems_Patch), nameof(WillTrade));

			FieldInfo traderField = AccessTools.Field(typeof(Dialog_SellableItems), nameof(Dialog_SellableItems.trader));

			foreach (CodeInstruction instruction in instructions)
			{
				if (instruction.Calls(vanillaWillTradeMethod))
				{
					yield return new CodeInstruction(OpCodes.Ldarg_0); // this
					yield return new CodeInstruction(OpCodes.Ldfld, traderField); // RimWorld.Dialog_SellableItems::trader
					yield return new CodeInstruction(OpCodes.Call, modifiedWillTradeMethod);
				}
				else
				{
					yield return instruction;
				}
			}
		}
	}
}