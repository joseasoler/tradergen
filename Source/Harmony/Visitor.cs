using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using TG.TraderKind;
using Verse;

namespace TG.Harmony
{
	[HarmonyPatch]
	public class Visitor
	{
		[HarmonyTranspiler]
		[HarmonyPatch(typeof(IncidentWorker_VisitorGroup),
			nameof(IncidentWorker_VisitorGroup.TryConvertOnePawnToSmallTrader))]
		public static IEnumerable<CodeInstruction> InjectModifiedTrader(IEnumerable<CodeInstruction> instructions)
		{
			var traderKindObtained = false;
			var traderKindInjected = false;
			foreach (var code in instructions)
			{
				if (!traderKindObtained && code.opcode == OpCodes.Call && code.operand is MethodInfo method)
				{
					if (method.ReturnParameter?.ParameterType == typeof(TraderKindDef))
					{
						traderKindObtained = true;
					}
				}

				yield return code;

				if (traderKindInjected || !traderKindObtained || code.opcode != OpCodes.Stloc_2)
				{
					continue;
				}

				// The first Stloc_2 after obtaining the traderKindDef is storing it into a local variable.
				// The new TraderKindDef can now be injected into that variable.

				// Set the previously generated TraderKindDef as an argument.
				yield return new CodeInstruction(opcode: OpCodes.Ldloc_2);
				// Set a seed of -1 as an argument so it can be generated randomly later.
				yield return new CodeInstruction(opcode: OpCodes.Ldc_I4_M1);
				// Set the map as an argument.
				yield return new CodeInstruction(opcode: OpCodes.Ldarg_3);
				// Set the faction as an argument.
				yield return new CodeInstruction(opcode: OpCodes.Ldarg_2);
				// Perform the call.
				yield return CodeInstruction.Call(typeof(Generator), nameof(Generator.Def),
					new[] {typeof(TraderKindDef), typeof(int), typeof(Map), typeof(Faction)});
				// Pop the result and assign it into the traderKindDef variable
				yield return new CodeInstruction(opcode: OpCodes.Stloc_2);
				// Disallow further modifications.
				traderKindInjected = true;
			}
		}
	}
}