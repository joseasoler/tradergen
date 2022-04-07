using System.Collections.Generic;
using System.Linq;
using RimWorld;
using TG.Next;
using Verse;

namespace TG
{
	/// <summary>
	/// Graph data structure traversed by the trader procedural generation algorithm.
	/// </summary>
	public class NodeDef : Def
	{
		/// <summary>
		/// List of generators added by selecting this node.
		/// </summary>
		public List<StockGenerator> generators = new List<StockGenerator>();

		/// <summary>
		/// Used to obtain the next set of nodes which must be evaluated.
		/// </summary>
		public Rule next;

		public override IEnumerable<string> ConfigErrors()
		{
			foreach (var error in base.ConfigErrors())
			{
				yield return error;
			}

			foreach (var error in generators.SelectMany(stockGenerator => stockGenerator.ConfigErrors(null)))
			{
				yield return error;
			}
		}
	}
}