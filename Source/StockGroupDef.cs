using System.Collections.Generic;
using RimWorld;
using Verse;

namespace TG
{
	/// <summary>
	/// Contains a group of stock generators for trading. Intended for use in LinkDef.
	/// </summary>
	public class StockGroupDef : Def
	{
		/// <summary>
		/// Generators contained in this group.
		/// </summary>
		public List<StockGenerator> generators;
	}
}
