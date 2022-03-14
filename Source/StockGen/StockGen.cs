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

		/// <summary>
		/// Stock generated by TG.StockGens may change depending on the original tile and faction of the trader.
		/// </summary>
		/// <param name="fromTile">Map tile considered as the origin of the trader.</param>
		/// <param name="faction">Faction of the trader.</param>
		public virtual void SetTraderInfo(in int fromTile, in Faction faction)
		{
		}
	}
}