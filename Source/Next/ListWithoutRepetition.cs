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
			var nextNodes = new List<NodeDef>();
			var times = num.RandomInRange;

			var remainingNodes = new List<NextNode>(nodes);
			var totalCommonality = remainingNodes.Sum(nextNode => nextNode.commonality);

			while (times > 0)
			{
				var currentCommonality = 0;
				var chosenCommonality = Rand.RangeInclusive(0, totalCommonality);

				NextNode chosenNode = null;
				foreach (var currentNode in remainingNodes)
				{
					currentCommonality += currentNode.commonality;
					if (currentCommonality < chosenCommonality) continue;
					chosenNode = currentNode;
					break;
				}

				if (chosenNode == null)
				{
					break;
				}

				// Nodes are chosen without repetition.
				remainingNodes.Remove(chosenNode);
				totalCommonality -= chosenNode.commonality;

				var nodeTimes = chosenNode.times.RandomInRange;
				while (nodeTimes > 0)
				{
					nextNodes.Add(chosenNode.node);
					--nodeTimes;
				}

				--times;
			}

			return nextNodes;
		}
	}
}