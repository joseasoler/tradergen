using System.Collections.Generic;
using System.Linq;
using RimWorld;
using TG.Gen;
using TG.Mod;
using Verse;

namespace TG.Trader
{
	/// <summary>
	/// Helper class for triggering orbital trader ship arrivals.
	/// </summary>
	public static class OrbitalTraderArrival
	{
		/// <summary>
		/// Checks if the colonists have a usable comms console in this map.
		/// </summary>
		/// <param name="map">Map in which an orbital trader is making an appearance.</param>
		/// <returns>True if there is a usable comms console in the map.</returns>
		private static bool ColonistsHavePoweredCommsConsole(in Map map)
		{
			return Enumerable.Any(map.listerBuildings.allBuildingsColonist,
				building => building.def.IsCommsConsole &&
				            (building.GetComp<CompPowerTrader>() == null || building.GetComp<CompPowerTrader>().PowerOn));
		}

		/// <summary>
		/// Generate the orbital trade ship arrival message for the player.
		/// </summary>
		/// <param name="ship">New orbital trade ship.</param>
		/// <param name="parms">Incident parameters.</param>
		private static void SendArrivalMessage(in TradeShip ship, in IncidentParms parms)
		{
			var factionText = ship.Faction == null
				? "TraderArrivalNoFaction".Translate()
				: "TraderArrivalFromFaction".Translate(ship.Faction.Named("FACTION"));
			var arrivalMessage = "TraderArrival".Translate(ship.name, ship.def.label, factionText);
			IncidentDefOf.OrbitalTraderArrival.Worker.SendStandardLetter(ship.def.LabelCap, arrivalMessage,
				LetterDefOf.PositiveEvent, parms,
				LookTargets.Invalid);
		}

		/// <summary>
		/// Obtain the Faction that the TradeShip will use.
		/// </summary>
		/// <param name="def">Procedurally generated trader definition.</param>
		/// <returns>Faction.</returns>
		public static Faction GetFaction(TraderKindDef def)
		{
			return Find.FactionManager.AllFactions.Where(faction => faction.def == def.faction)
				.TryRandomElement(out var result)
				? result
				: null;
		}

		private static TradeShip CreateTradeShip(in TraderGenDef genDef)
		{
			if (genDef == null)
			{
				// The TradeShip constructor is already patched to generate its own random TraderKindDef.
				return new TradeShip(null);
			}

			// Create a TradeShip using a specific TraderGenDef. Avoid using the patched parameter constructor.
			var ship = new TradeShip();
			ship.def = Find.World.GetComponent<TraderKind>().Generate(genDef);
			ship.faction = GetFaction(ship.def);
			ship.things = new ThingOwner<Thing>(ship);
			// ToDo procedural generation of ship names.
			ship.name = NameGenerator.GenerateName(RulePackDefOf.NamerTraderGeneral, new List<string>());
			if (ship.faction != null)
			{
				ship.name = $"{ship.name} {"OfLower".Translate()} {ship.faction.Name}";
			}

			ship.loadID = Find.UniqueIDsManager.GetNextPassingShipID();

			return ship;
		}

		public static void Arrive(in IncidentParms parms, in TraderGenDef genDef)
		{
			var map = (Map) parms.target;

			if (map == null || map.passingShipManager.passingShips.Count >= Settings.MaxOrbitalShips)
			{
				return;
			}

			var ship = CreateTradeShip(genDef);

			if (ColonistsHavePoweredCommsConsole(map))
			{
				parms.traderKind = ship.def;
				SendArrivalMessage(ship, parms);
			}

			map.passingShipManager.AddShip(ship);

			ship.GenerateThings();
		}
	}
}