using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;

namespace TG.Next
{
	/// <summary>
	/// Defines rules for choosing the next nodes to evaluate.
	/// </summary>
	public abstract class Rule
	{
		/// <summary>
		/// Evaluates and obtains the list of nodes to evaluate next.
		/// </summary>
		/// <param name="nodeDef">Node definition currently in use.</param>
		/// <param name="fromTile">Map tile considered as the origin of the trader.</param>
		/// <param name="faction">Faction of the trader.</param>
		/// <returns>List of nodes to evaluate next.</returns>
		public abstract List<NodeDef> Nodes(in NodeDef nodeDef, in int fromTile = -1, in Faction faction = null);
	}
}