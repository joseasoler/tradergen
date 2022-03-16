using System.Collections.Generic;
using RimWorld;
using Verse;

namespace TG.Next
{
	/// <summary>
	/// Chooses num nodes to use, without repetition.
	/// </summary>
	public class ChooseN : Rule
	{
		/// <summary>
		/// Number of nodes to be chosen from the list.
		/// </summary>
		public IntRange num = IntRange.one;

		/// <summary>
		/// Provided group of nodes.
		/// </summary>
		public List<NodeChoice> nodes;

		public override List<NodeDef> Nodes(in NodeDef nodeDef, in int fromTile = -1, in Faction faction = null)
		{
			var chosenNextNodes = Algorithm.ChooseNWeightedRandomly(nodes, x => x.commonality, num.RandomInRange);
			var nodeDefs = new List<NodeDef>();

			foreach (var chosenNextNode in chosenNextNodes)
			{
				var nodeTimes = chosenNextNode.times.RandomInRange;
				while (nodeTimes > 0)
				{
					nodeDefs.Add(chosenNextNode.node);
					--nodeTimes;
				}
			}

			return nodeDefs;
		}
	}
}