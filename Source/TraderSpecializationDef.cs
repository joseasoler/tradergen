using System.Collections.Generic;
using RimWorld;
using Verse;

namespace TG
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
	}
}