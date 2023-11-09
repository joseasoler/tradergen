using System.Collections.Generic;
using RimWorld;
using TraderGen.DefOfs;
using TraderGen.StockModification;
using Verse;

namespace TraderGen.StockGen
{
	public class HemogenicGenepack : CustomGenepack
	{
		private static List<XenotypeDef> _hemogenicXenotypes;

		private static HashSet<GeneDef> _hemogenicGenes;

		private static float XenotypeWeight(XenotypeDef def)
		{
			if (def == XenotypeDefOf.Sanguophage && def.factionlessGenerationWeight <= 0.0F)
			{
				// Although sanguophages cannot be found as factionless pawns, they are allowed as an exception as they are
				// intended to be usual owners of orbital traders with the bloodfeeder specialization.
				return 1.0F;
			}
	
			bool isHemogenic = false;
			foreach (GeneDef geneDef in def.genes)
			{
				if (_hemogenicGenes.Contains(geneDef))
				{
					isHemogenic = true;
				}
			}
			if (!isHemogenic)
			{
				return 0.0f;
			}

			if (def.factionlessGenerationWeight > 0.0F)
			{
				// For other modded bloodfeeders, their factionless generation weight is respected.
				return 0.5F * def.factionlessGenerationWeight;
			}

			return 0.1F;
		}

		private static void Initialize()
		{
			if (_hemogenicXenotypes != null)
			{
				return;
			}

			_hemogenicGenes = new HashSet<GeneDef>() { GeneDefOf.Hemogenic };
			if (GeneDefOfs.VU_WhiteRoseBite != null)
			{
				_hemogenicGenes.Add(GeneDefOfs.VU_WhiteRoseBite);
			}

			_hemogenicXenotypes = new List<XenotypeDef>();
			foreach (XenotypeDef xenotypeDef in DefDatabase<XenotypeDef>.AllDefsListForReading)
			{
				if (XenotypeWeight(xenotypeDef) > 0.0f)
				{
					_hemogenicXenotypes.Add(xenotypeDef);
				}
			}
		}

		protected override bool SetCustomGeneration()
		{
			Initialize();
			if (_hemogenicXenotypes == null || _hemogenicXenotypes.Count == 0)
			{
				Logger.ErrorOnce("TraderGen.StockGen.HemogenicGenepack could not find any hemogenic xenotypes.");
				return false;
			}

			var chosenXenotype = Algorithm.ChooseNWeightedRandomly(_hemogenicXenotypes, XenotypeWeight, 1);
			if (chosenXenotype.Count == 0)
			{
				Logger.ErrorOnce("TraderGen.StockGen.HemogenicGenepack could not randomly choose a hemogenic xenotype.");
				return false;
			}

			CustomGenepackGenerator.Xenotype = chosenXenotype[0];
			CustomGenepackGenerator.Name = CustomGenepackGenerator.Xenotype.label;
			return true;
		}
	}
}