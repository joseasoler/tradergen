using Verse;

namespace TG
{
	/// <summary>
	/// Represents information about a specific node choice in some rules.
	/// </summary>
	public class NextNode
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