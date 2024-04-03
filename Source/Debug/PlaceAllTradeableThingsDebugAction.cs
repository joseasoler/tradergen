using System.Collections.Generic;
using LudeonTK;
using RimWorld;
using Verse;

namespace TraderGen.Debug
{
	/// <summary>
	/// Place all things and animals which can be sold or purchased in the map.
	/// </summary>
	public static class PlaceAllTradeableThingsDebugAction
	{
		private static bool ShouldGenerateThing(ThingDef thingDef)
		{
			return
				// Only items and buildings.
				(thingDef.category == ThingCategory.Item || thingDef.category == ThingCategory.Building) &&
				// Which can be traded.
				thingDef.tradeability != Tradeability.None &&
				// Prevent buildings which are not minifiable.
				(thingDef.building == null || thingDef.Minifiable) &&
				// Disable minified things.
				thingDef.thingClass != typeof(MinifiedThing) &&
				// Disable minified trees.
				thingDef.thingClass != typeof(MinifiedTree);
		}

		/// <summary>
		/// Races having a comp with any of these names are not generated
		/// </summary>
		private static readonly HashSet<string> IgnoreRaceComps = new HashSet<string>
		{
			// In Vanilla Psycasts Expanded, this includes creatures linked to the caster such as constructs.
			"CompBreakLink",
			// In Vanilla Expanded Framework, this means animals which cannot be tamed in any way.
			"CompUntameable"
		};

		private static bool ShouldGeneratePawn(PawnKindDef pawnKindDef)
		{
			if (pawnKindDef.race?.race == null || !pawnKindDef.race.race.Animal ||
			    pawnKindDef.race.tradeability == Tradeability.None)
			{
				return false;
			}

			if (pawnKindDef.race.comps == null)
			{
				return true;
			}

			for (int compIndex = 0; compIndex < pawnKindDef.race.comps.Count; ++compIndex)
			{
				if (IgnoreRaceComps.Contains(pawnKindDef.race.comps[compIndex].compClass.Name))
				{
					return false;
				}
			}

			return true;
		}

		[DebugAction(DebugActions.DebugCategory, allowedGameStates = AllowedGameStates.PlayingOnMap)]
		public static void PlaceAllTradeableThings()
		{
			IntVec3 center = Find.CurrentMap.Center;
			List<ThingDef> thingDefs = DefDatabase<ThingDef>.AllDefsListForReading;
			for (int thingDefIndex = 0; thingDefIndex < thingDefs.Count; ++thingDefIndex)
			{
				ThingDef thingDef = thingDefs[thingDefIndex];
				if (!ShouldGenerateThing(thingDef))
				{
					continue;
				}

				DebugThingPlaceHelper.DebugSpawn(thingDef, center, 1);
			}

			List<PawnKindDef> pawnKindDefs = DefDatabase<PawnKindDef>.AllDefsListForReading;

			for (int pawnKindDefIndex = 0; pawnKindDefIndex < pawnKindDefs.Count; ++pawnKindDefIndex)
			{
				PawnKindDef pawnKindDef = pawnKindDefs[pawnKindDefIndex];

				if (!ShouldGeneratePawn(pawnKindDef))
				{
					continue;
				}

				var newPawn = PawnGenerator.GeneratePawn(pawnKindDef, Faction.OfPlayer);
				GenSpawn.Spawn(newPawn, center, Find.CurrentMap);
			}
		}
	}
}