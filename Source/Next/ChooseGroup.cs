using System.Collections.Generic;
using RimWorld;
using Verse;

namespace TG.Next
{
	/// <summary>
	/// Choose a single group of nodes.
	/// </summary>
	public class ChooseGroup : Rule
	{
		/// <summary>
		/// List of available node groups to choose from.
		/// </summary>
		public List<GroupChoice> nodeGroups = new List<GroupChoice>();

		public override List<NodeDef> Nodes(in NodeDef nodeDef, in int fromTile = -1, in Faction faction = null)
		{
			var result = nodeGroups.TryRandomElementByWeight(x => x.commonality, out var nextGroup);
			return result ? nextGroup.nodes : new List<NodeDef>();
		}
	}
}