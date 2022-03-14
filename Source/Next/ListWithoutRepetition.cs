using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace TG.Next
{
	/// <summary>
	/// Chooses num nodes to use, without repetition, from a list.
	/// </summary>
	public class ListWithoutRepetition : Rule
	{
		/// <summary>
		/// Number of nodes to be chosen from the list.
		/// </summary>
		public IntRange num = IntRange.one;

		/// <summary>
		/// Provided group of nodes.
		/// </summary>
		public List<NextNode> nodes;

		public override List<NodeDef> Nodes(in NodeDef nodeDef, in BiomeDef biomeDef = null, in Faction faction = null)
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