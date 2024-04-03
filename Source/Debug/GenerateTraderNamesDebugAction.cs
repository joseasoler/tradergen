using System.Collections.Generic;
using System.Text;
using LudeonTK;
using RimWorld;
using TraderGen.TraderKind;
using Verse;

namespace TraderGen.Debug
{
	/// <summary>
	/// Generate a large number of trader names for a specific orbital trader type.
	/// </summary>
	public static class GenerateTraderNamesDebugAction
	{
		private static void LogTraderNames(TraderKindDef traderKindDef)
		{
			const int count = 50;
			StringBuilder sb = new StringBuilder();
			sb.AppendLine($"Generating {count} random names for {traderKindDef.label}:");
			for (int nameIndex = 0; nameIndex < count; ++nameIndex)
			{
				sb.AppendLine($"\t{Cache.Name(Rand.Int, traderKindDef, null)}");
			}

			Logger.Message(sb.ToString());
		}

		[DebugAction(DebugActions.DebugCategory, allowedGameStates = AllowedGameStates.Playing)]
		public static void GenerateTraderNames()
		{
			List<TraderKindDef> traderKindDefs = DefDatabase<TraderKindDef>.AllDefsListForReading;
			List<DebugMenuOption> menuOptions = new List<DebugMenuOption>();
			for (int traderKindDefIndex = 0; traderKindDefIndex < traderKindDefs.Count; ++traderKindDefIndex)
			{
				TraderKindDef traderKindDef = traderKindDefs[traderKindDefIndex];
				if (!traderKindDef.orbital)
				{
					continue;
				}

				menuOptions.Add(new DebugMenuOption(traderKindDef.label, DebugMenuOptionMode.Action,
					() => LogTraderNames(traderKindDef)));
			}

			Find.WindowStack.Add(new Dialog_DebugOptionListLister(menuOptions));
		}
	}
}