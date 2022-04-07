using System;
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

		/// <summary>
		/// Sort DebugMenuOptions by label.
		/// </summary>
		/// <param name="a">First menu option.</param>
		/// <param name="b">Second menu option.</param>
		/// <returns></returns>
		private static int CompareDebugMenuOptions(DebugMenuOption a, DebugMenuOption b)
		{
			return string.Compare(a.label, b.label, StringComparison.Ordinal);
		}

		/// <summary>
		/// Generates a new orbital trade ship with a NodeDef chosen by the user.
		/// </summary>
		[DebugAction("TraderGen", allowedGameStates = AllowedGameStates.PlayingOnMap)]
		private static void GenerateOrbitalTrader()
		{
			var debugOrbitalTrader = DefDatabase<TraderKindDef>.GetNamed("TG_OrbitalDebug");

			var nodeDefsOptions = DefDatabase<NodeDef>.AllDefs
				.Select(nodeDef =>
					new DebugMenuOption(nodeDef.defName, DebugMenuOptionMode.Action, () =>
					{
						debugOrbitalTrader.GetModExtension<GenExtension>().node = nodeDef;
						Find.CurrentMap.passingShipManager.DebugSendAllShipsAway();
						IncidentDefOf.OrbitalTraderArrival.Worker.TryExecute(new IncidentParms
						{
							target = Find.CurrentMap,
							traderKind = debugOrbitalTrader
						});
					})).ToList();
			nodeDefsOptions.Sort(CompareDebugMenuOptions);
			Find.WindowStack.Add(new Dialog_DebugOptionListLister(nodeDefsOptions));
		}
	}
}