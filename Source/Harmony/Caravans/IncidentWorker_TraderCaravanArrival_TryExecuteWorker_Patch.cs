using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using TraderGen.TraderKind;
using Verse;

namespace TraderGen.Harmony.Caravans
{
	[HarmonyPatch(typeof(IncidentWorker_TraderCaravanArrival),
		nameof(IncidentWorker_TraderCaravanArrival.TryExecuteWorker))]
	internal static class IncidentWorker_TraderCaravanArrival_TryExecuteWorker_Patch
	{
		private static void ReplaceLetter(IncidentWorker_TraderCaravanArrival instance, IncidentParms parms,
			List<Pawn> pawns, TraderKindDef traderKind)
		{
			Pawn trader = null;
			foreach (Pawn pawn in pawns)
			{
				if (pawn.TraderKind != null)
				{
					trader = pawn;
					break;
				}
			}

			if (trader == null)
			{
				Logger.Error($"Could not generate caravan arrival letter for {traderKind}");
				return;
			}

			TaggedString letterLabel = "LetterLabelTraderCaravanArrival"
				.Translate((NamedArgument)parms.faction.Name, Util.LabelWithTraderSpecialization(trader).CapitalizeFirst())
				.CapitalizeFirst();
			TaggedString letterText =
				"LetterTraderCaravanArrival".Translate(parms.faction.NameColored, (NamedArgument)traderKind.label)
					.CapitalizeFirst() + ("\n\n" + "LetterCaravanArrivalCommonWarning".Translate());
			PawnRelationUtility.Notify_PawnsSeenByPlayer_Letter(pawns, ref letterLabel, ref letterText,
				"LetterRelatedPawnsNeutralGroup".Translate((NamedArgument)Faction.OfPlayer.def.pawnsPlural), true);
			instance.SendStandardLetter(letterLabel, letterText, LetterDefOf.PositiveEvent, parms,
				(Thing)trader);
		}

		internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var letterMethod = AccessTools.Method(typeof(IncidentWorker_TraderCaravanArrival), "SendLetter");

			foreach (var code in instructions)
			{
				if (code.opcode == OpCodes.Callvirt && code.operand is MethodInfo info && info == letterMethod)
				{
					yield return new CodeInstruction(OpCodes.Call,
						AccessTools.Method(typeof(IncidentWorker_TraderCaravanArrival_TryExecuteWorker_Patch),
							nameof(ReplaceLetter)));
				}
				else
				{
					yield return code;
				}
			}
		}
	}
}