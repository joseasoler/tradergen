using System.Collections.Generic;
using RimWorld;

namespace TG.Next
{
	public class All : Rule
	{
		public List<NodeDef> nodes = new List<NodeDef>();

		public override List<NodeDef> Nodes(in NodeDef nodeDef, in int fromTile = -1, in Faction faction = null)
		{
			return nodes;
		}
	}
}