using System.Collections.Generic;
using RimWorld;
using TG.Mod;
using Verse;

namespace TG.StockGen
{
	/// <summary>
	/// Adds silver to the stock of orbital traders. The amount can be scaled using mod settings.
	/// </summary>
	public class OrbitalSilver : StockGenerator
	{
		/// <summary>
		/// Add silver to the stock, scaled by the OrbitalSilverScaling setting.
		/// </summary>
		/// <param name="forTile">Tile in which the transaction takes place.</param>
		/// <param name="faction">Faction of the trader.</param>
		/// <returns></returns>
		public override IEnumerable<Thing> GenerateThings(int forTile, Faction faction = null)
		{
			return StockGeneratorUtility.TryMakeForStock(ThingDefOf.Silver,
				(int) (Settings.OrbitalSilverScaling * RandomCountOf(ThingDefOf.Silver) / 100.0f), faction);
		}

		public override bool HandlesThingDef(ThingDef thingDef)
		{
			return thingDef == ThingDefOf.Silver;
		}
	}
}