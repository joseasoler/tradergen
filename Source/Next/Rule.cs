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
		public abstract List<NodeDef> Nodes(in NodeDef nodeDef, in BiomeDef biomeDef = null, in Faction faction = null);
	}
}