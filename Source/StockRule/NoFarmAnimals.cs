using Verse;

namespace TraderGen.StockRule
{
	/// <summary>
	/// Prevents selling and purchasing farm animals
	/// </summary>
	public class NoFarmAnimals : Rule
	{
		public override bool ForbidsTrading(in ThingDef def)
		{
			return def.race != null && def.tradeTags != null && def.tradeTags.Contains("AnimalFarm");
		}

		public override bool ForbidsStocking(in ThingDef def)
		{
			return def.race != null && def.tradeTags != null && def.tradeTags.Contains("AnimalFarm");
		}
	}
}