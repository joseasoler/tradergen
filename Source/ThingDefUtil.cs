using RimWorld;
using Verse;

namespace TG
{
	/// <summary>
	/// Provides utility functions for performing common ThingDef checks.
	/// </summary>
	public class ThingDefUtil
	{
		/// <summary>
		/// Armor rating stat threshold that must be reached before an apparel is considered armor.
		/// </summary>
		private const float ArmorThreshold = 0.15f;

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

			var stuff = GenStuff.DefaultStuffFor(def);
			return def.GetStatValueAbstract(StatDefOf.ArmorRating_Blunt, stuff) > ArmorThreshold ||
			       def.GetStatValueAbstract(StatDefOf.ArmorRating_Sharp, stuff) > ArmorThreshold;
		}
	}
}