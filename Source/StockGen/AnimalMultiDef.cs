using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace TG.StockGen
{
	/// <summary>
	/// Sells kindCountRange random types of animals in pawnKindDefs, with countRange/totalPriceRange in stock for each.
	/// Takes into account totalPriceRange and customCountRanges, which are ignored by vanilla StockGenerator_Animals.
	/// </summary>
	public class AnimalMultiDef : StockGenerator
	{
		public List<PawnKindDef> pawnKindDefs;

		public IntRange kindCountRange = new IntRange(1, 1);

		private HashSet<ThingDef> _cachedThingDefs;

		public override IEnumerable<Thing> GenerateThings(int forTile, Faction faction = null)
		{
			var chosenPawnDefs = Algorithm.ChooseNWeightedRandomly(pawnKindDefs,
				def => StockGenerator_Animals.SelectionChanceFromWildnessCurve.Evaluate(def.RaceProps.wildness),
				kindCountRange.RandomInRange);

			foreach (var pawnDef in chosenPawnDefs)
			{
				var count = RandomCountOf(pawnDef.race);
				for (var i = 0; i < count; ++i)
				{
					yield return PawnGenerator.GeneratePawn(new PawnGenerationRequest(pawnDef, tile: forTile));
				}
			}
		}

		public override bool HandlesThingDef(ThingDef thingDef)
		{
			if (thingDef.category != ThingCategory.Pawn || !thingDef.race.Animal)
			{
				return false;
			}

			if (_cachedThingDefs == null)
			{
				_cachedThingDefs = new HashSet<ThingDef>(pawnKindDefs.Select(pawnKindDef => pawnKindDef.race));
			}

			return _cachedThingDefs.Contains(thingDef);
		}
	}
}