using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using TG.TraderKind;
using Verse;

namespace TG.Harmony
{
	[HarmonyPatch]
	public class Caravan
	{
		/// <summary>
		/// Injects a modified TraderKindDef into the caravan generation process.
		/// </summary>
		[HarmonyTranspiler]
		[HarmonyPatch(typeof(PawnGroupKindWorker_Trader), nameof(PawnGroupKindWorker_Trader.GeneratePawns))]
		private static IEnumerable<CodeInstruction> InjectModifiedTrader(IEnumerable<CodeInstruction> instructions)
		{
			var traderKindInjected = false;
			foreach (var code in instructions)
			{
				yield return code;

				// GeneratePawns stores the trader pawn using Stloc_1.
				// The injection point is right after that call.
				if (traderKindInjected || code.opcode != OpCodes.Stloc_1)
				{
					continue;
				}

				// traderKindDef is the first argument of Generator.Def.
				yield return new CodeInstruction(OpCodes.Ldloc_0);

				// Set the chosen deterministic generation seed.
				// Load the pawn as an argument.
				yield return new CodeInstruction(OpCodes.Ldloc_1);
				// Obtain the random price factor seed. The result is set as the second argument of Generator.Def.
				yield return new CodeInstruction(OpCodes.Call,
					AccessTools.Property(typeof(Pawn), nameof(Pawn.RandomPriceFactorSeed)).GetGetMethod());

				// Load the parms object.
				yield return new CodeInstruction(OpCodes.Ldarg_1);
				// Load the map tile as the third argument of Generator.Def.
				yield return new CodeInstruction(OpCodes.Ldfld,
					AccessTools.Field(typeof(PawnGroupMakerParms), nameof(PawnGroupMakerParms.tile)));

				// Load the parms object.
				yield return new CodeInstruction(OpCodes.Ldarg_1);
				// Load the faction as the fourth and final argument of Generator.Def.
				yield return new CodeInstruction(OpCodes.Ldfld,
					AccessTools.Field(typeof(PawnGroupMakerParms), nameof(PawnGroupMakerParms.faction)));

				// Perform the Generator.Def call.
				yield return CodeInstruction.Call(typeof(Generator), nameof(Generator.Def),
					new[] {typeof(TraderKindDef), typeof(int), typeof(int), typeof(Faction)});
				// Pop the result and assign it into the traderKindDef variable
				yield return new CodeInstruction(OpCodes.Stloc_0);

				// At this point, the pawn trader tracker still holds the template traderKindDef.

				// Load the pawn as an argument.
				yield return new CodeInstruction(OpCodes.Ldloc_1);
				// Load the pawn trader tracker.
				yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Pawn), nameof(Pawn.trader)));
				// Load the traderKindDef.
				yield return new CodeInstruction(OpCodes.Ldloc_0);
				// Store it into the pawn trader tracker.
				yield return new CodeInstruction(OpCodes.Stfld,
					AccessTools.Field(typeof(Pawn_TraderTracker), nameof(Pawn_TraderTracker.traderKind)));

				// Disallow further modifications.
				traderKindInjected = true;
			}
		}
	}
}