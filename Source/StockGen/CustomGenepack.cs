using System.Collections.Generic;
using RimWorld;
using TG.Harmony;
using Verse;

namespace TG.StockGen
{
	public abstract class CustomGenepack : ConditionMatcher
	{
		public IntRange architeGenes = IntRange.zero;

		public IntRange nonArchiteGenes = IntRange.zero;
		
		public override IEnumerable<string> ConfigErrors(TraderKindDef parentDef)
		{
			foreach (var err in base.ConfigErrors(parentDef))
			{
				yield return err;
			}

			if (architeGenes == IntRange.zero && nonArchiteGenes == IntRange.zero)
			{
				yield return "TG.StockGen.CustomGenepack: cannot generate genepack without genes.";
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
			
			foreach (var result in base.GenerateThings(forTile, faction))
			{
				yield return result;
			}

			CustomGenepackGenerator.Reset();
		}
	}
}