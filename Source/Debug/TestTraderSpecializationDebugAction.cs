using System.Collections.Generic;
using LudeonTK;
using RimWorld;
using TraderGen.DefOfs;
using Verse;

namespace TraderGen.Debug
{
	/// <summary>
	/// Generates a new orbital trader with a specialization chosen by the user.
	/// </summary>
	public static class TestTraderSpecializationDebugAction
	{
		private static void GenerateTraderWithSpecialization(TraderSpecializationDef specializationDef)
		{
			// Override the specialization of the debug orbital trader.
			TraderKindDef debugOrbitalTraderKindDef = TraderKindDefs.TG_OrbitalDebug;
			GenExtension modExtension = debugOrbitalTraderKindDef.GetModExtension<GenExtension>();
			modExtension.specializations[0].def = specializationDef;

			// Generate the new trader.
			Find.CurrentMap.passingShipManager.DebugSendAllShipsAway();
			IncidentDefOf.OrbitalTraderArrival.Worker.TryExecute(new IncidentParms
			{
				target = Find.CurrentMap,
				traderKind = debugOrbitalTraderKindDef
			});
		}

		[DebugAction(DebugActions.DebugCategory, allowedGameStates = AllowedGameStates.PlayingOnMap)]
		public static void TestTraderSpecialization()
		{
			List<TraderSpecializationDef> traderSpecializationDefs =
				DefDatabase<TraderSpecializationDef>.AllDefsListForReading;
			List<DebugMenuOption> menuOptions = new List<DebugMenuOption>();
			for (int specializationIndex = 0; specializationIndex < traderSpecializationDefs.Count; ++specializationIndex)
			{
				TraderSpecializationDef specializationDef = traderSpecializationDefs[specializationIndex];
				menuOptions.Add(new DebugMenuOption(specializationDef.defName, DebugMenuOptionMode.Action,
					() => GenerateTraderWithSpecialization(specializationDef)));
			}

			menuOptions.Sort(DebugActions.CompareDebugMenuOptions);
			Find.WindowStack.Add(new Dialog_DebugOptionListLister(menuOptions));
		}
	}
}