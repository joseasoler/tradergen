using System.Linq;
using RimWorld;
using TG.DefOf;
using Verse;

namespace TG.Things
{
	/// <summary>
	/// Utility functions for dealing with Things and ThingDefs
	/// </summary>
	public class Util
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

		/// <summary>
		/// Checks if provided def is an alcoholic drink.
		/// </summary>
		/// <param name="def">Provided ThingDef</param>
		/// <returns>True if def is an alcoholic drink.</returns>
		public static bool IsAlcohol(in ThingDef def)
		{
			if (def.ingestible?.outcomeDoers == null || def.ingestible.JoyKind != JoyKind.Chemical ||
			    !def.ingestible.foodType.HasFlag(FoodTypeFlags.Liquor)) return false;

			return (from outcomeDoer in def.ingestible.outcomeDoers
				where outcomeDoer.GetType() == typeof(IngestionOutcomeDoer_GiveHediff)
				select (IngestionOutcomeDoer_GiveHediff) outcomeDoer).Any(o =>
				o.hediffDef?.hediffClass == typeof(Hediff_Alcohol));
		}
	}
}