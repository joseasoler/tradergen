using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace TG.Ideo
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
		/// Contains rules which may prevent stock from being purchased, generated or sold.
		/// </summary>
		public List<Rule.Rule> stockRules = new List<Rule.Rule>();

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

			foreach (var error in visitorStockGens.SelectMany(stockGenerator => stockGenerator.ConfigErrors(null)))
			{
				yield return error;
			}

			foreach (var error in traderStockGens.SelectMany(stockGenerator => stockGenerator.ConfigErrors(null)))
			{
				yield return error;
			}

			foreach (var error in settlementStockGens.SelectMany(stockGenerator => stockGenerator.ConfigErrors(null)))
			{
				yield return error;
			}

			foreach (var error in stockRules.SelectMany(rule => rule.ConfigErrors()))
			{
				yield return error;
			}
		}
	}
}