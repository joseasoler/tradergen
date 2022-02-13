using System.Collections.Generic;
using RimWorld;
using TG.Mod;
using Verse;

namespace TG.StockGen
{
	public class OrbitalSilver : StockGenerator_SingleDef
	{
		public override IEnumerable<Thing> GenerateThings(int forTile, Faction faction = null)
		{
			return StockGeneratorUtility.TryMakeForStock(ThingDefOf.Silver,
				(int) (Settings.OrbitalSilverScaling * RandomCountOf(ThingDefOf.Silver) / 100.0f), faction);
		}
	}
}