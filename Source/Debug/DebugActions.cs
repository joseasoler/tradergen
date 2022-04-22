using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using TG.TraderKind;
using Verse;

namespace TG.Debug
{
	public class DebugActions
	{
		[DebugAction("TraderGen", allowedGameStates = AllowedGameStates.PlayingOnMap)]
		private static void PlaceAllTradeableThings()
		{
			foreach (var def in DefDatabase<ThingDef>.AllDefs)
			{
				if (def.category != ThingCategory.Item && def.category != ThingCategory.Building ||
				    def.tradeability == Tradeability.None || def.building != null && !def.Minifiable ||
				    def.thingClass == typeof(MinifiedThing) || def.thingClass == typeof(MinifiedTree) ||
				    // One of the items with this comp causes null pointer exceptions.
				    def.comps != null && def.comps.Any(comp => comp.compClass.Name == "CompGlowerExtended"))
				{
					continue;
				}

				DebugThingPlaceHelper.DebugSpawn(def, UI.MouseCell(), 1);
			}

			foreach (var def in DefDatabase<PawnKindDef>.AllDefs)
			{
				if (!def.race.race.Animal || def.race.tradeability == Tradeability.None ||
				    // Prevent untamable animals from VE/AA from appearing.
				    def.race.comps != null &&
				    def.race.comps.Any(comp => comp.compClass.Name == "CompUntameable")
				   )
				{
					continue;
				}

				var newPawn = PawnGenerator.GeneratePawn(def, Faction.OfPlayer);
				GenSpawn.Spawn(newPawn, UI.MouseCell(), Find.CurrentMap);
			}
		}

		[DebugAction("TraderGen", allowedGameStates = AllowedGameStates.Playing)]
		private static void GenerateTraderNames()
		{
			var options = DefDatabase<TraderKindDef>.AllDefs.Where(t => t.orbital)
				.Select(traderKindDef => new DebugMenuOption(traderKindDef.label, DebugMenuOptionMode.Action, () =>
				{
					Logger.Gen($"{traderKindDef.label}:");
					for (var index = 0; index < 50; ++index)
					{
						Logger.Gen($"\t{Generator.Name(traderKindDef, null)}");
					}
				}))
				.ToList();
			Find.WindowStack.Add(new Dialog_DebugOptionListLister(options));
		}
	}
}