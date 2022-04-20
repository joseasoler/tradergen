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
		/// Some TG.StockGens may need to perform extra initialization before each generation.
		/// </summary>
		/// <param name="fromTile">Tile in which the transaction will take place.</param>
		/// <param name="faction">Faction of the trader.</param>
		public virtual void BeforeGen(in int fromTile, in Faction faction)
		{
		}
	}
}