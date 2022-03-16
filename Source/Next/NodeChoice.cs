using Verse;

namespace TG.Next
{
	/// <summary>
	/// Represents information about a specific choice in some rules.
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