using Verse;

namespace TG.TraderKind
{
	/// <summary>
	/// Provides alternate data for generating trader stock.
	/// StockGenerators defined in the TraderKindDef will be ignored in favor of the ones in the chosen nodes.
	/// </summary>
	public class GenExtension : DefModExtension
	{
		/// <summary>
		/// Starting point of the procedural generation algorithm.
		/// </summary>
		public NodeDef node;
	}
}