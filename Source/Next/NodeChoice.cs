using Verse;

namespace TG.Next
{
	/// <summary>
	/// Choose a single node, with a specific commonality, and apply it a random amount of times.
	/// </summary>
	public class NodeChoice
	{
		/// <summary>
		/// Node to be chosen.
		/// </summary>
		public NodeDef node;

		/// <summary>
		/// Commonality of this choice.
		/// </summary>
		public int commonality = 1;

		/// <summary>
		/// Number of times that this node will be added to the pending queue.
		/// </summary>
		public IntRange times = IntRange.one;
	}
}