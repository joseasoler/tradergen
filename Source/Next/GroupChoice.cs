using System.Collections.Generic;

namespace TG.Next
{
	/// <summary>
	/// Choose a single group of nodes with a given commonality.
	/// </summary>
	public class GroupChoice
	{
		/// <summary>
		/// Nodes to be chosen.
		/// </summary>
		public List<NodeDef> nodes;

		/// <summary>
		/// Commonality of this choice.
		/// </summary>
		public int commonality = 1;
	}
}