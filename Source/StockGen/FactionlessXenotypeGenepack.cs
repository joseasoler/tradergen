using System.Collections.Generic;
using RimWorld;
using TraderGen.StockModification;
using Verse;

namespace TraderGen.StockGen
{
	public class FactionlessXenotypeGenepack : CustomGenepack
	{
		private static List<XenotypeDef> _factionlessXenotypes;

		private static readonly HashSet<string> DisallowedXenotypes = new HashSet<string>
		{
			// Alpha Genes special random custom xenotype.
			"AG_RandomCustom"
		};

		private static bool HasDisallowedGene(XenotypeDef def)
		{
			foreach (GeneDef geneDef in def.genes)
			{
				// Hemogenic xenotypes have their own stock generator.
				if (geneDef == GeneDefOf.Hemogenic)
				{
					return true;
				}

				// Omit android "genes" from Vanilla Races Expanded - Androids.
				var categoryName = geneDef.displayCategory.defName;
				if (categoryName == "VREA_Hardware" || categoryName == "VREA_Subroutine")
				{
					return true;
				}
			}

			return false;
		}

		private static bool DisallowedXenotype(XenotypeDef def)
		{
			return def == XenotypeDefOf.Baseliner || DisallowedXenotypes.Contains(def.defName) || HasDisallowedGene(def);
		}

		private static float XenotypeWeight(XenotypeDef def)
		{
			return DisallowedXenotype(def) ? 0.0F : 2.0F * def.factionlessGenerationWeight;
		}

		private static void Initialize()
		{
			if (_factionlessXenotypes != null)
			{
				return;
			}

			_factionlessXenotypes = new List<XenotypeDef>();
			foreach (XenotypeDef xenotypeDef in DefDatabase<XenotypeDef>.AllDefsListForReading)
			{
				if (XenotypeWeight(xenotypeDef) > 0.0f)
				{
					_factionlessXenotypes.Add(xenotypeDef);
				}
			}
		}

		protected override bool SetCustomGeneration()
		{
			Initialize();
			if (_factionlessXenotypes == null || _factionlessXenotypes.Count == 0)
			{
				Logger.ErrorOnce("TraderGen.StockGen.FactionlessXenotypeGenepack could not find any valid xenotypes.");
				return false;
			}

			var chosenXenotype = Algorithm.ChooseNWeightedRandomly(_factionlessXenotypes, XenotypeWeight, 1);
			if (chosenXenotype.Count == 0)
			{
				Logger.ErrorOnce("TraderGen.StockGen.FactionlessXenotypeGenepack could not randomly choose a valid xenotype.");
				return false;
			}

			CustomGenepackGenerator.Xenotype = chosenXenotype[0];
			CustomGenepackGenerator.Name = CustomGenepackGenerator.Xenotype.label;
			return true;
		}
	}
}