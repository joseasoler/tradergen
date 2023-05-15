using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using HarmonyLib;
using RimWorld;
using TraderGen.TraderKind;
using Verse;

namespace TraderGen.Harmony
{
	public static class CaravanArrival
	{
		public static void Patch(HarmonyLib.Harmony harmony)
		{
			// The transpiler is used to remove the vanilla letter.
			// The postfix finds the pawn that acts as the trader, and generates a proper TraderGen letter.
			var caravanArrival = AccessTools.Method(typeof(IncidentWorker_TraderCaravanArrival),
				nameof(IncidentWorker_TraderCaravanArrival.TryExecuteWorker));
			var replaceLetter =
				new HarmonyMethod(AccessTools.Method(typeof(CaravanArrival), nameof(TranspileLabel)));
			harmony.Patch(caravanArrival, transpiler: replaceLetter);
		}

		private static void ReplaceLetter(IncidentWorker_TraderCaravanArrival instance,
			IncidentParms parms,
			List<Pawn> pawns,
			TraderKindDef traderKind)
		{
			var trader = pawns.Find(pawn => pawn.TraderKind != null);

			var letterLabel = "LetterLabelTraderCaravanArrival"
				.Translate((NamedArgument) parms.faction.Name, Util.Label(trader).CapitalizeFirst()).CapitalizeFirst();
			var letterText =
				"LetterTraderCaravanArrival".Translate(parms.faction.NameColored, (NamedArgument) traderKind.label)
					.CapitalizeFirst() + ("\n\n" + "LetterCaravanArrivalCommonWarning".Translate());
			PawnRelationUtility.Notify_PawnsSeenByPlayer_Letter(pawns, ref letterLabel, ref letterText,
				"LetterRelatedPawnsNeutralGroup".Translate((NamedArgument) Faction.OfPlayer.def.pawnsPlural), true);
			instance.SendStandardLetter(letterLabel, letterText, LetterDefOf.PositiveEvent, parms,
				(Thing) pawns[0]);
		}

		private static IEnumerable<CodeInstruction> TranspileLabel(IEnumerable<CodeInstruction> instructions)
		{
			var letterMethod = AccessTools.Method(typeof(IncidentWorker_TraderCaravanArrival), "SendLetter");

			foreach (var code in instructions)
			{
				if (code.opcode == OpCodes.Callvirt && code.operand is MethodInfo info && info == letterMethod)
				{
					yield return new CodeInstruction(OpCodes.Call,
						AccessTools.Method(typeof(CaravanArrival), nameof(ReplaceLetter)));
				}
				else
				{
					yield return code;
				}
			}
		}
	}
}