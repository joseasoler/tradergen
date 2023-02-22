using System.Collections.Generic;
using RimWorld;
using TG.Harmony;
using Verse;

namespace TG.StockGen
{
	public class FactionlessXenotypeGenepack : CustomGenepack
	{
		private static List<XenotypeDef> _factionlessXenotypes;

		private static float XenotypeWeight(XenotypeDef def)
		{
			return def == XenotypeDefOf.Baseliner ? 0.0f : 2.0f * def.factionlessGenerationWeight;
		}

		private static void Initialize()
		{
			if (_factionlessXenotypes != null)
			{
				return;
			}

			_factionlessXenotypes = new List<XenotypeDef>();
			foreach (var xenotypeDef in DefDatabase<XenotypeDef>.AllDefs)
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
				Logger.ErrorOnce("TG.StockGen.FactionlessXenotypeGenepack could not find any valid xenotypes.");
				return false;
			}

			var chosenXenotype = Algorithm.ChooseNWeightedRandomly(_factionlessXenotypes, XenotypeWeight, 1);
			if (chosenXenotype.Count == 0)
			{
				Logger.ErrorOnce("TG.StockGen.FactionlessXenotypeGenepack could not randomly choose a valid xenotype.");
				return false;
			}

			CustomGenepackGenerator.Xenotype = chosenXenotype[0];
			CustomGenepackGenerator.Name = CustomGenepackGenerator.Xenotype.label;
			return true;
		}
	}
}