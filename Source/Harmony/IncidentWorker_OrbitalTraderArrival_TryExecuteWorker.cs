using System;
using System.Linq;
using HarmonyLib;
using RimWorld;
using TG.Trader;
using Verse;

namespace TG.Harmony
{
	/// <summary>
	/// Overrides IncidentWorker_OrbitalTraderArrival to hand generation over to ProceduralTradeShip.
	/// </summary>
	[HarmonyPatch(typeof(IncidentWorker_OrbitalTraderArrival), "TryExecuteWorker")]
	public class IncidentWorker_OrbitalTraderArrival_TryExecuteWorker
	{
		/// <summary>
		/// Checks if the colonists have a usable comms console in this map.
		/// </summary>
		/// <param name="map">Map in which an orbital trader is making an appearance.</param>
		/// <returns>True if there is a powered comms console in the map.</returns>
		private static bool ColonistsHavePoweredCommsConsole(Map map)
		{
			return Enumerable.Any(map.listerBuildings.allBuildingsColonist,
				building => building.def.IsCommsConsole &&
				            (building.GetComp<CompPowerTrader>() == null || building.GetComp<CompPowerTrader>().PowerOn));
		}

		/// <summary>
		/// Hands over the TraderKindDef generation to ProceduralTradeShip.
		/// </summary>
		/// <param name="__result">True if an orbital trader was generated successfully.</param>
		/// <param name="parms">Incident generation parameters.</param>
		/// <returns>Always false, as this prefix takes over vanilla orbital trader generation.</returns>
		[HarmonyPrefix]
		static bool Prefix(ref bool __result, IncidentParms parms)
		{
			var map = (Map) parms.target;

			if (map == null || !ColonistsHavePoweredCommsConsole(map) ||
			    map.passingShipManager.passingShips.Count >= IncidentWorker_OrbitalTraderArrival.MaxShips)
			{
				__result = false;
			}
			else
			{
				if (!DefDatabase<TraderGenDef>.AllDefs.Where(traderGenDef => traderGenDef.orbital)
					    .TryRandomElementByWeight(traderGenDef => traderGenDef.commonality, out var genDef))
				{
					throw new InvalidOperationException("Could not find a valid TraderGenDef to generate orbital traders.");
				}

				DoOrbitalTraderArrival(genDef, parms);
				__result = true;
			}

			return false;
		}

		public static void DoOrbitalTraderArrival(in TraderGenDef genDef, IncidentParms parms)
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