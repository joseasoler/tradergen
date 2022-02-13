using System.Collections.Generic;
using System.Linq;
using Verse;

namespace TG
{
	/// <summary>
	/// Markov chain link used for procedural generation of traders.
	/// </summary>
	public class LinkDef : Def
	{
		/// <summary>
		/// List of stock groups added by selecting this LinkDef.
		/// </summary>
		public List<StockGroupDef> stockGroups;

		/// <summary>
		/// Links to choose randomly from in order to continue the chain. An empty list means an end to the chain. 
		/// </summary>
		public List<Link> next;

		/// <summary>
		/// Cached value for TotalCommonality.
		/// </summary>
		private int _cachedTotalCommonality = -1;

		/// <summary>
		/// Accumulated commonality of all next links.
		/// </summary>
		public int TotalCommonality
		{
			get
			{
				if (_cachedTotalCommonality < 0)
				{
					_cachedTotalCommonality = next?.Sum(link => link.commonality) ?? 0;
				}

				return _cachedTotalCommonality;
			}
		}
	}
}
