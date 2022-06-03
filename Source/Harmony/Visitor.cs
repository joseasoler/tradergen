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
			var traderKindInjected = false;
			foreach (var code in instructions)
			{
				yield return code;

				// TryConvertOnePawnToSmallTrader stores the chosen TraderKindDef using Stloc_2.
				// The injection point is right after that call.
				if (traderKindInjected || code.opcode != OpCodes.Stloc_2)
				{
					continue;
				}
	
				// Set the previously generated TraderKindDef as an argument.
				yield return new CodeInstruction(opcode: OpCodes.Ldloc_2);

				// Set the chosen deterministic generation seed.
				// Load the pawn as an argument.
				yield return new CodeInstruction(opcode: OpCodes.Ldloc_0);
				// Obtain the random price factor seed. The result is set as the second argument.
				yield return new CodeInstruction(OpCodes.Call,
					AccessTools.Property(typeof(Pawn), nameof(Pawn.RandomPriceFactorSeed)).GetGetMethod());

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