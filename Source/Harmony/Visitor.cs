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
		/// <summary>
		/// Inject a modified TraderKindDef into the visitor trader generation process.
		/// </summary>
		/// <param name="instructions">Original set of instructions.</param>
		/// <returns>New set of instructions.</returns>
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

				// traderKindDef is the first argument of Generator.Def.
				yield return new CodeInstruction(OpCodes.Ldloc_2);

				// Set the chosen deterministic generation seed.
				// Load the pawn as an argument.
				yield return new CodeInstruction(OpCodes.Ldloc_0);
				// Obtain the random price factor seed. The result is set as the second argument of Generator.Def.
				yield return new CodeInstruction(OpCodes.Call,
					AccessTools.Property(typeof(Pawn), nameof(Pawn.RandomPriceFactorSeed)).GetGetMethod());

				// Load the pawn as the third argument of Generator.Def.
				yield return new CodeInstruction(OpCodes.Ldloc_0);

				// Perform the Generator.Def call.
				yield return CodeInstruction.Call(typeof(Generator), nameof(Generator.Def),
					new[] {typeof(TraderKindDef), typeof(int), typeof(Pawn)});
				// Pop the result and assign it into the traderKindDef variable
				yield return new CodeInstruction(OpCodes.Stloc_2);

				// Disallow further modifications.
				traderKindInjected = true;
			}
		}
	}
}