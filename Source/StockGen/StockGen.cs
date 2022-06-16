using System.Text;
using RimWorld;

namespace TG.StockGen
{
	/// <summary>
	/// Parent class for all TG StockGenerators.
	/// </summary>
	public abstract class StockGen : StockGenerator
	{
		/// <summary>
		/// Sets maxTechLevelGenerate to the tech level of the StockGen's faction if it has any.
		/// </summary>
		public bool forceFactionTechLimits /* = false */;

		public virtual void ToText(ref StringBuilder b)
		{
			if (forceFactionTechLimits)
			{
				b.Append($"forceFactionTechLimits: {forceFactionTechLimits}\n");
			}
		}

		/// <summary>
		/// Some TG.StockGens may need to perform extra initialization before each generation.
		/// </summary>
		/// <param name="tile">Tile in which the transaction will take place.</param>
		/// <param name="faction">Faction of the trader.</param>
		public virtual void BeforeGen(in int tile, in Faction faction)
		{
			if (forceFactionTechLimits && faction != null)
			{
				maxTechLevelGenerate = faction.def.techLevel;
			}
		}
	}
}