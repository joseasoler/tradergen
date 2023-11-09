using System.Collections.Generic;
using RimWorld;
using TraderGen.StockModification;
using Verse;

namespace TraderGen.StockGen
{
	public abstract class CustomGenepack : ConditionMatcher
	{
		public IntRange architeGenes = IntRange.zero;

		public IntRange nonArchiteGenes = IntRange.zero;

		public CustomGenepack()
		{
			thingDefCountRange = IntRange.one;
		}

		public override IEnumerable<string> ConfigErrors(TraderKindDef parentDef)
		{
			foreach (string error in base.ConfigErrors(parentDef))
			{
				yield return error;
			}

			if (architeGenes == IntRange.zero && nonArchiteGenes == IntRange.zero)
			{
				yield return "TraderGen.StockGen.CustomGenepack: cannot generate genepack without genes.";
			}
		}

		protected override bool CanBuy(in ThingDef def)
		{
			return def == ThingDefOf.Genepack;
		}

		protected abstract bool SetCustomGeneration();

		public override IEnumerable<Thing> GenerateThings(int forTile, Faction faction = null)
		{
			if (!SetCustomGeneration())
			{
				yield break;
			}

			CustomGenepackGenerator.ArchiteGeneCount = architeGenes;
			CustomGenepackGenerator.NonArchiteGeneCount = nonArchiteGenes;

			foreach (Thing result in base.GenerateThings(forTile, faction))
			{
				yield return result;
			}

			CustomGenepackGenerator.Reset();
		}
	}
}