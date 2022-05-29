using System;
using System.Linq;
using Force.DeepCloner;
using RimWorld;
using TG.TraderKind;
using Verse;

namespace TG.Debug
{
	public class DebugActions
	{
		/// <summary>
		/// Category to use for TraderGen debug actions.
		/// </summary>
		private const string DebugCategory = "TraderGen";

		/// <summary>
		/// Place all things and animals which can be sold or purchased in the map.
		/// </summary>
		[DebugAction(DebugCategory, allowedGameStates = AllowedGameStates.PlayingOnMap)]
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
		/// Generate a large number of trader names for a specific orbital trader type.
		/// </summary>
		[DebugAction(DebugCategory, allowedGameStates = AllowedGameStates.Playing)]
		private static void GenerateTraderNames()
		{
			var options = DefDatabase<TraderKindDef>.AllDefs.Where(t => t.orbital)
				.Select(traderKindDef => new DebugMenuOption(traderKindDef.label, DebugMenuOptionMode.Action, () =>
				{
					Logger.Message($"{traderKindDef.label}:");
					for (var index = 0; index < 50; ++index)
					{
						Logger.Message($"\t{Generator.Name(traderKindDef, null)}");
					}
				}))
				.ToList();
			Find.WindowStack.Add(new Dialog_DebugOptionListLister(options));
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
		/// Generates a new orbital trader with a specialization chosen by the user.
		/// </summary>
		[DebugAction(DebugCategory, allowedGameStates = AllowedGameStates.PlayingOnMap)]
		private static void TestTraderSpecialization()
		{
			var debugOrbitalTrader = DefDatabase<TraderKindDef>.GetNamed("TG_OrbitalDebug");

			var specializationOptions = DefDatabase<TraderSpecializationDef>.AllDefs
				.Select(specializationDef =>
					new DebugMenuOption(specializationDef.defName, DebugMenuOptionMode.Action, () =>
					{
						debugOrbitalTrader.GetModExtension<GenExtension>().specializations.First().def = specializationDef;
						Find.CurrentMap.passingShipManager.DebugSendAllShipsAway();
						IncidentDefOf.OrbitalTraderArrival.Worker.TryExecute(new IncidentParms
						{
							target = Find.CurrentMap,
							traderKind = debugOrbitalTrader
						});
					})).ToList();
			specializationOptions.Sort(CompareDebugMenuOptions);
			Find.WindowStack.Add(new Dialog_DebugOptionListLister(specializationOptions));
		}

		/// <summary>
		/// Adds an orbital trader using the stock of any TraderKindDef even if they are not orbital.
		/// </summary>
		[DebugAction(DebugCategory, null, false, false, allowedGameStates = AllowedGameStates.PlayingOnMap)]
		private static void TestAnyTrader()
		{
			var traderOptions = DefDatabase<TraderKindDef>.AllDefs.Select(def => new DebugMenuOption(def.defName,
					DebugMenuOptionMode.Action, () =>
					{
						Find.CurrentMap.passingShipManager.DebugSendAllShipsAway();

						var newDef = def.ShallowClone();

						// IncidentWorker_OrbitalTraderArrival uses this to set the faction. IncidentParms.faction is ignored.
						// But IncidentParms.faction is used in other cases such as when generating stock.
						newDef.faction = Util.GetFactionDef(def);
						newDef.orbital = true;
						if (string.IsNullOrEmpty(newDef.label))
						{
							newDef.label = newDef.defName;
						}

						var factionParam = newDef.faction == null
							? null
							: Find.FactionManager.AllFactions.FirstOrDefault(f => f.def == newDef.faction);
						IncidentDefOf.OrbitalTraderArrival.Worker.TryExecute(new IncidentParms
							{target = Find.CurrentMap, traderKind = newDef, faction = factionParam});
					}))
				.ToList();

			traderOptions.Sort(CompareDebugMenuOptions);
			Find.WindowStack.Add(new Dialog_DebugOptionListLister(traderOptions));
		}
	}
}