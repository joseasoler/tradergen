using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using TraderGen.TraderKind;
using Verse;

namespace TraderGen.Harmony.Orbital
{
	[HarmonyPatch(typeof(IncidentWorker_OrbitalTraderArrival),
		nameof(IncidentWorker_OrbitalTraderArrival.TryExecuteWorker))]
	public static class IncidentWorker_OrbitalTraderArrival_TryExecuteWorker_Patch
	{
		private static TaggedString OrbitalTraderLetterLabel(TaggedString _, TradeShip trader)
		{
			return Util.LabelWithTraderSpecialization(trader).CapitalizeFirst();
		}

		public static TaggedString OrbitalTraderLetterText(TaggedString _, TradeShip trader)
		{
			var name = trader.name;
			var label = trader.def.label;
			var factionStr = trader.Faction == null
				? "TraderArrivalNoFaction".Translate()
				: "TraderArrivalFromFaction".Translate(trader.Faction.Named("FACTION"));

			var specializations = Cache.Specializations(trader);

			TaggedString letterText;
			if (specializations.NullOrEmpty())
			{
				letterText = "TraderArrival".Translate(name, label, factionStr);
			}
			else
			{
				var specializationsStr = "";
				for (var index = 0; index < specializations.Count - 1; ++index)
				{
					if (index > 0)
					{
						specializationsStr += ", ";
					}

					specializationsStr += specializations[index].label;
				}

				if (specializations.Count > 1)
				{
					specializationsStr += ' ' + "AndLower".Translate() + ' ';
				}

				specializationsStr += specializations[specializations.Count - 1].label;

				letterText = "TraderArrivalSpecializations".Translate(name, label, specializationsStr, factionStr);
			}

			return letterText;
		}

		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo originalLabelGetter = AccessTools.PropertyGetter(typeof(Def), nameof(Def.LabelCap));

			MethodInfo modifiedLabelMethod = AccessTools.Method(
				typeof(IncidentWorker_OrbitalTraderArrival_TryExecuteWorker_Patch),
				nameof(OrbitalTraderLetterLabel));

			FieldInfo positiveEventField = AccessTools.Field(typeof(LetterDefOf), nameof(LetterDefOf.PositiveEvent));

			MethodInfo modifiedTextMethod = AccessTools.Method(
				typeof(IncidentWorker_OrbitalTraderArrival_TryExecuteWorker_Patch),
				nameof(OrbitalTraderLetterText));

			foreach (CodeInstruction instruction in instructions)
			{
				if (instruction.opcode == OpCodes.Ldsfld && instruction.operand as FieldInfo == positiveEventField)
				{
					yield return new CodeInstruction(OpCodes.Ldloc_1); // vis (TradeShip)
					yield return new CodeInstruction(OpCodes.Call, modifiedTextMethod);
				}

				yield return instruction;

				if (instruction.Calls(originalLabelGetter))
				{
					yield return new CodeInstruction(OpCodes.Ldloc_1); // vis (TradeShip)
					yield return new CodeInstruction(OpCodes.Call, modifiedLabelMethod);
				}
			}
		}
	}
}