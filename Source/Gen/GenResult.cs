using System.Collections.Generic;
using RimWorld;

namespace TG.Gen
{
	/// <summary>
	/// Results of the TraderGen random generation algorithm.
	/// </summary>
	public class GenResult
	{
		/// <summary>
		/// List of stock generators to use.
		/// </summary>
		public List<StockGenerator> generators = new List<StockGenerator>();
	}
}