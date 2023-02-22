using System.Collections.Generic;
using RimWorld;
using TG.Harmony;
using Verse;

namespace TG.StockGen
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
				return 1.0f;
			}
			
			// For other modded bloodfeeders, their factionless generation weight is respected.
			return 2.0f * def.factionlessGenerationWeight;
		}

		private static void Initialize()
		{
			if (_hemogenicXenotypes != null)
			{
				return;
			}

			_hemogenicXenotypes = new List<XenotypeDef>();
			foreach (var xenotypeDef in DefDatabase<XenotypeDef>.AllDefs)
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
				Logger.ErrorOnce("TG.StockGen.HemogenicGenepack could not find any hemogenic xenotypes.");
				return false;
			}

			var chosenXenotype = Algorithm.ChooseNWeightedRandomly(_hemogenicXenotypes, XenotypeWeight, 1);
			if (chosenXenotype.Count == 0)
			{
				Logger.ErrorOnce("TG.StockGen.HemogenicGenepack could not randomly choose a hemogenic xenotype.");
				return false;
			}

			CustomGenepackGenerator.Xenotype = chosenXenotype[0];
			CustomGenepackGenerator.Name = CustomGenepackGenerator.Xenotype.label;
			return true;
		}
	}
}