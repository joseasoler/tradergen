using System.Collections.Generic;
using RimWorld;
using Verse;

namespace TraderGen.Ideo
{
	/// <summary>
	/// Associates a precept with the modifications in stock that it should perform in traders following it.
	/// </summary>
	public class PreceptGenDef : Def
	{
		/// <summary>
		/// Stock generators added to visitors by this precept.
		/// </summary>
		public List<StockGenerator> visitorStockGens = new List<StockGenerator>();

		/// <summary>
		/// Stock generators added to caravans and orbital traders by this precept.
		/// </summary>
		public List<StockGenerator> traderStockGens = new List<StockGenerator>();

		/// <summary>
		/// Stock generators added to settlements by this precept.
		/// </summary>
		public List<StockGenerator> settlementStockGens = new List<StockGenerator>();

		/// <summary>
		/// Contains rules which may prevent stock from being traded or generated..
		/// </summary>
		public List<StockRule.Rule> stockRules = new List<StockRule.Rule>();

		/// <summary>
		/// Displays all XML configuration errors.
		/// </summary>
		/// <returns>Errors triggered by the data contained in this structure.</returns>
		public override IEnumerable<string> ConfigErrors()
		{
			foreach (var error in base.ConfigErrors())
			{
				yield return error;
			}

			foreach (StockGenerator stockGen in visitorStockGens)
			{
				foreach (string error in stockGen.ConfigErrors(null))
				{
					yield return error;
				}
			}

			foreach (StockGenerator stockGen in traderStockGens)
			{
				foreach (string error in stockGen.ConfigErrors(null))
				{
					yield return error;
				}
			}

			foreach (StockGenerator stockGen in settlementStockGens)
			{
				foreach (string error in stockGen.ConfigErrors(null))
				{
					yield return error;
				}
			}

			foreach (StockRule.Rule rule in stockRules)
			{
				foreach (string error in rule.ConfigErrors())
				{
					yield return error;
				}
			}
		}
	}
}