using HarmonyLib;
using RimWorld;
using TraderGen.StockModification;
using Verse;

namespace TraderGen.Harmony
{
	[HarmonyPatch(typeof(GeneUtility), nameof(GeneUtility.GenerateGeneSet))]
	internal static class GeneUtility_GenerateGeneSet_Patch
	{
		internal static bool Prefix(ref GeneSet __result, int? seed)
		{
			if (!ModsConfig.BiotechActive || CustomGenepackGenerator.Xenotype == null)
			{
				return true;
			}

			if (seed.HasValue)
			{
				Rand.PushState(seed.Value);
			}

			__result = CustomGenepackGenerator.GenerateFromXenotype();

			if (seed.HasValue)
			{
				Rand.PopState();
			}

			if (CustomGenepackGenerator.Name == null)
			{
				__result.GenerateName();
			}
			else
			{
				__result.name =
					$"{ThingDefOf.Genepack.label} ({CustomGenepackGenerator.Name.ToLower()}, {__result.genes.Count})";
			}

			__result.SortGenes();

			return false;
		}
	}
}