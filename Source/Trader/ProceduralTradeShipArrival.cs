using System;
using System.Linq;
using RimWorld;
using Verse;

namespace TG.Trader
{
	public class ProceduralTradeShipArrival
	{
		/// <summary>
		/// Checks if the colonists have a usable comms console in this map.
		/// </summary>
		/// <param name="map">Map in which an orbital trader is making an appearance.</param>
		/// <returns>True if there is a powered comms console in the map.</returns>
		private static bool ColonistsHavePoweredCommsConsole(in Map map)
		{
			return Enumerable.Any(map.listerBuildings.allBuildingsColonist,
				building => building.def.IsCommsConsole &&
				            (building.GetComp<CompPowerTrader>() == null || building.GetComp<CompPowerTrader>().PowerOn));
		}

		/// <summary>
		/// Try to trigger the arrival of a ProceduralTradeShip. 
		/// </summary>
		/// <param name="parms">Incident generation parameters.</param>
		/// <returns>True if it was possible to generate a new ProceduralTradeShip.</returns>
		public static bool TryArrival(in IncidentParms parms)
		{
			var map = (Map) parms.target;

			if (map == null || !ColonistsHavePoweredCommsConsole(map) ||
			    map.passingShipManager.passingShips.Count >= IncidentWorker_OrbitalTraderArrival.MaxShips)
			{
				return false;
			}

			if (!DefDatabase<TraderGenDef>.AllDefs.Where(traderGenDef => traderGenDef.orbital)
				    .TryRandomElementByWeight(traderGenDef => traderGenDef.commonality, out var genDef))
			{
				throw new InvalidOperationException("Could not find a valid TraderGenDef to generate orbital traders.");
			}

			DoArrival(genDef, parms);
			return true;
		}

		/// <summary>
		/// Generate a new orbital trade ship and trigger its arrival.
		/// </summary>
		/// <param name="genDef">Template used to generate the orbital trade ship.</param>
		/// <param name="parms">Incident generation parameters.</param>
		public static void DoArrival(in TraderGenDef genDef, in IncidentParms parms)
		{
			var map = (Map) parms.target;
			var ship = new ProceduralTradeShip(genDef, Math.Abs(Rand.Int));
			if (ColonistsHavePoweredCommsConsole(map))
			{
				var factionText = ship.Faction == null
					? "TraderArrivalNoFaction".Translate()
					: "TraderArrivalFromFaction".Translate(ship.Faction.Named("FACTION"));
				var arrivalMessage = "TraderArrival".Translate(ship.name, ship.def.label, factionText);
				IncidentDefOf.OrbitalTraderArrival.Worker.SendStandardLetter(ship.def.LabelCap, arrivalMessage,
					LetterDefOf.PositiveEvent, parms,
					LookTargets.Invalid);
			}

			map.passingShipManager.AddShip(ship);
			ship.GenerateThings();
		}
	}
}