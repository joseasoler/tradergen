using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace TraderGen
{
	/// <summary>
	/// Defines a trader specialization. May also be used for categories.
	/// </summary>
	public class TraderSpecializationDef : Def
	{
		/// <summary>
		/// Stock generators added by this specialization.
		/// </summary>
		public List<StockGenerator> stockGens = new List<StockGenerator>();

		public override IEnumerable<string> ConfigErrors()
		{
			foreach (var error in base.ConfigErrors())
			{
				yield return error;
			}

			foreach (var error in stockGens.SelectMany(stockGenerator => stockGenerator.ConfigErrors(null)))
			{
				yield return error;
			}
		}
	}
}