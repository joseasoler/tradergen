using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace TraderGen.Harmony
{
	[HarmonyPatch(typeof(GeneUtility), nameof(GeneUtility.GenerateGeneSet))]
	public static class CustomGenepackGenerator
	{
		/// <summary>
		/// Set this variable to make the next genepack generation contain only genes from this xenotype.
		/// </summary>
		public static XenotypeDef Xenotype;

		/// <summary>
		/// Number of archite genes to generate.
		/// </summary>
		public static IntRange ArchiteGeneCount = IntRange.zero;

		/// <summary>
		/// Number of non-archite genes to generate.
		/// </summary>
		public static IntRange NonArchiteGeneCount = IntRange.zero;

		/// <summary>
		/// Override the name of the genepack.
		/// </summary>
		public static string Name;

		/// <summary>
		/// Reset all state variables so the next generation can take place normally.
		/// </summary>
		public static void Reset()
		{
			Xenotype = null;
			ArchiteGeneCount = IntRange.zero;
			NonArchiteGeneCount = IntRange.zero;
			Name = null;
		}

		/// <summary>
		/// Defines genes that should never appear in generated genepacks.
		/// </summary>
		/// <param name="def">Gene being checked.</param>
		/// <returns>True if the gene can be used.</returns>
		private static bool CanAddGene(GeneDef def)
		{
			return def.canGenerateInGeneSet
			       // Alpha Genes are not intended to appear in genepacks.
			       && !def.defName.StartsWith("AG_");
		}

		/// <summary>
		/// Utility function for adding a set of genes along with their prerequisites.
		/// </summary>
		/// <param name="set">Gene set being processed.</param>
		/// <param name="genes">List of genes to be added.</param>
		private static void AddGenesWithPrerequisites(ref GeneSet set, List<GeneDef> genes)
		{
			foreach (var gene in genes)
			{
				if (gene.prerequisite != null && !set.genes.Contains(gene.prerequisite) &&
				    set.CanAddGeneDuringGeneration(gene.prerequisite))
				{
					set.AddGene(gene.prerequisite);
				}

				if (set.CanAddGeneDuringGeneration(gene))
				{
					set.AddGene(gene);
				}
			}
		}

		/// <summary>
		/// Generate a genepack with genes from a specific xenotype.
		/// </summary>
		/// <returns>Geneset for the pack.</returns>
		private static GeneSet GenerateFromXenotype()
		{
			// Collect genes from the xenotype that could be used in a genepack.
			var architeGenes = new List<GeneDef>();
			var nonArchiteGenes = new List<GeneDef>();
			foreach (var gene in Xenotype.genes)
			{
				if (!CanAddGene(gene))
				{
					continue;
				}

				if (gene.biostatArc > 0)
				{
					architeGenes.Add(gene);
				}
				else
				{
					nonArchiteGenes.Add(gene);
				}
			}

			var architeCount = ArchiteGeneCount.RandomInRange;
			var nonArchiteCount = NonArchiteGeneCount.RandomInRange;

			var result = new GeneSet();
			var chosenArchiteGenes =
				Algorithm.ChooseNWeightedRandomly(architeGenes, x => 0.01f + x.selectionWeight, architeCount);
			AddGenesWithPrerequisites(ref result, chosenArchiteGenes);
			var chosenNonArchiteGenes =
				Algorithm.ChooseNWeightedRandomly(nonArchiteGenes, x => 0.01f + x.selectionWeight, nonArchiteCount);
			AddGenesWithPrerequisites(ref result, chosenNonArchiteGenes);

			return result;
		}

		internal static bool Prefix(ref GeneSet __result, int? seed)
		{
			if (!ModsConfig.BiotechActive || Xenotype == null)
			{
				return true;
			}

			if (seed.HasValue)
			{
				Rand.PushState(seed.Value);
			}

			__result = GenerateFromXenotype();

			if (seed.HasValue)
			{
				Rand.PopState();
			}

			if (Name == null)
			{
				__result.GenerateName();
			}
			else
			{
				__result.name = $"{ThingDefOf.Genepack.label} ({Name.ToLower()}, {__result.genes.Count})";
			}
			__result.SortGenes();

			return false;
		}
	}
}