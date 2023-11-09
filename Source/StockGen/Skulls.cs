using System.Collections.Generic;
using RimWorld;
using Verse;

namespace TraderGen.StockGen
{
	/// <summary>
	/// Purchases and sells skulls. Each skull gets a person name from a random culture and gender. Requires Ideology.
	/// </summary>
	public class Skulls : ConditionMatcher
	{
		public Skulls()
		{
			thingDefCountRange = IntRange.one;
		}

		protected override IEnumerable<Thing> TryMakeForStock(ThingDef thingDef, Faction faction)
		{
			if (!ModsConfig.IdeologyActive)
			{
				yield break;
			}

			for (var idx = 0; idx < RandomCountOf(ThingDefOf.Skull); ++idx)
			{
				var thing = ThingMaker.MakeThing(ThingDefOf.Skull);
				thing.stackCount = 1;
				var sourceComp = thing.TryGetComp<CompHasSources>();
				var name = PawnBioAndNameGenerator.GenerateFullPawnName(ThingDefOf.Human,
					primaryCulture: DefDatabase<CultureDef>.AllDefsListForReading.RandomElement(),
					gender: Rand.Value < 0.5 ? Gender.Male : Gender.Female);
				sourceComp.AddSource(name.ToStringShort);

				yield return thing;
			}
		}

		protected override bool CanBuy(in ThingDef def)
		{
			return ModsConfig.IdeologyActive && def == ThingDefOf.Skull;
		}
	}
}