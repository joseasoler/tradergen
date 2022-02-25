using System.Collections.Generic;
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
		/// List of stock groups added by selecting this node.
		/// </summary>
		public List<StockGroupDef> stockGroups;

		/// <summary>
		/// Used to obtain the next set of nodes which must be evaluated.
		/// </summary>
		public Rule next;
	}
}