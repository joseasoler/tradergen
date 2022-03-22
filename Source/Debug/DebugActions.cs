using RimWorld;
using Verse;

namespace TG.Debug
{
	public class DebugActions
	{
		[DebugAction("TraderGen", null, allowedGameStates = AllowedGameStates.PlayingOnMap)]
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
				    // Prevent untameable animals from VE/AA from appearing.
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
	}
}