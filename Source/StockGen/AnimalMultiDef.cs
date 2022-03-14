using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace TG.StockGen
{
	/// <summary>
	/// Sells kindCountRange random types of animals in pawnKindDefs, with countRange/totalPriceRange in stock for each.
	/// Uses the same random weight depending on wildness as StockGenerator_Animals.
	/// Takes into account totalPriceRange and customCountRanges, which are ignored by vanilla StockGenerator_Animals.
	/// </summary>
	public class AnimalMultiDef : StockGen
	{
		public List<PawnKindDef> pawnKindDefs;

		public IntRange kindCountRange = new IntRange(1, 1);

		public bool newborn = false;

		private HashSet<ThingDef> _cachedThingDefs;

		public override IEnumerable<string> ConfigErrors(TraderKindDef parentDef)
		{
			foreach (var err in base.ConfigErrors(parentDef))
			{
				yield return err;
			}

			foreach (var pawnKindDef in pawnKindDefs.Where(pawnKindDef =>
				         pawnKindDef.race?.race == null || !pawnKindDef.race.race.Animal))
			{
				yield return $"TG.StockGen.AnimalMultiDef: {pawnKindDef} is not an animal.";
			}
		}

		public override void ToText(ref StringBuilder b)
		{
			b.Append($"pawnKindDefs: {string.Join(", ", pawnKindDefs)}\n");
			b.Append($"kindCountRange: {kindCountRange}\n");
			b.Append($"newborn: {newborn}\n");
		}

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
					yield return PawnGenerator.GeneratePawn(new PawnGenerationRequest(pawnDef, tile: forTile, newborn: newborn));
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