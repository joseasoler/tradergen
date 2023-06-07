using System.Collections.Generic;
using RimWorld;
using TraderGen.Harmony;
using Verse;

namespace TraderGen.StockGen
{
	public class HemogenicGenepack : CustomGenepack
	{
		private static List<XenotypeDef> _hemogenicXenotypes;

		private static float XenotypeWeight(XenotypeDef def)
		{
			if (!def.genes.Contains(GeneDefOf.Hemogenic))
			{
				return 0.0f;
			}

			if (def == XenotypeDefOf.Sanguophage)
			{
				// Although sanguophages cannot be found as factionless pawns, they are allowed as an exception as they are
				// intended to be usual owners of orbital traders with the bloodfeeder specialization.
				return 1.0F;
			}

			if (def.factionlessGenerationWeight > 0)
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

			_hemogenicXenotypes = new List<XenotypeDef>();
			foreach (var xenotypeDef in DefDatabase<XenotypeDef>.AllDefsListForReading)
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