using System.Collections.Generic;
using System.Text;
using RimWorld;

namespace TG.StockGen
{
	/// <summary>
	/// Parent class for all TG StockGenerators.
	/// </summary>
	public abstract class StockGen : StockGenerator
	{
		public abstract void ToText(ref StringBuilder b);
	}
}