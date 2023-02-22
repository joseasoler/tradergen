using System.Collections.Generic;
using RimWorld;
using TG.Harmony;
using Verse;

namespace TG.StockGen
{
	public class HemogenicGenepack : CustomGenepack
	{
		private static List<XenotypeDef> _hemogenicXenotypes;

		public HemogenicGenepack()
		{
			thingDefCountRange = IntRange.one;
		}

		/// <summary>
		/// Sanguophages have more probability to be chosen, but all xenotypes with hemogenic can appear.
		/// </summary>
		/// <param name="def">Hemogenic xenotype to check</param>
		/// <returns>Random weight of the xenotype.</returns>
		private static float HemogenicXenotypeWeight(XenotypeDef def)
		{
			var value = 1.0f / (1 + _hemogenicXenotypes.Count);
			if (def == XenotypeDefOf.Sanguophage)
			{
				value += 0.3f;
			}

			return value;
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
				if (xenotypeDef.genes.Contains(GeneDefOf.Hemogenic))
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

			var chosenXenotype = Algorithm.ChooseNWeightedRandomly(_hemogenicXenotypes, HemogenicXenotypeWeight, 1);
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