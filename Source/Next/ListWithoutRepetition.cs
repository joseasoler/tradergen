using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace TG.Next
{
	public class ListWithoutRepetition : Rule
	{
		/// <summary>
		/// Number of nodes to be chosen from the list.
		/// </summary>
		public IntRange num = IntRange.one;

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