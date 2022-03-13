using RimWorld;
using Verse;

namespace TG.StockGen
{
	/// <summary>
	/// Utility functions used by different StockGens.
	/// </summary>
	public static class Util
	{
		/// <summary>
		/// Returns true if the provided def should be considered armor when generating trader stock.
		/// </summary>
		/// <param name="def">Provided ThingDef</param>
		/// <returns>True if the apparel is armor.</returns>
		public static bool IsArmor(in ThingDef def)
		{
			if (!def.IsApparel)
			{
				return false;
			}

			// Armor rating stat threshold that must be reached before an apparel is considered armor.
			// See RimWorld.StockGenerator_Clothes.
			const float armorThreshold = 0.15f;

			var stuff = GenStuff.DefaultStuffFor(def);
			return def.GetStatValueAbstract(StatDefOf.ArmorRating_Blunt, stuff) > armorThreshold ||
			       def.GetStatValueAbstract(StatDefOf.ArmorRating_Sharp, stuff) > armorThreshold;
		}
	}
}