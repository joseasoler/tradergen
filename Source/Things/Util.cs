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

		private static readonly FoodTypeFlags[] vegan =
			{FoodTypeFlags.VegetableOrFruit, FoodTypeFlags.Seed};

		/// <summary>
		/// Checks if the def is a type of raw vegan food. Ignores fungus as that is handled by other precept.
		/// </summary>
		/// <param name="def">Provided ThingDef</param>
		/// <returns>True for all raw vegan foods.</returns>
		public static bool IsRawVegan(ThingDef def)
		{
			return def.IsIngestible && vegan.Any(flag => def.ingestible.foodType.HasFlag(flag));
		}

		/// <summary>
		/// Checks if the def is a type of meat which is not human or insect meat.
		/// </summary>
		/// <param name="def">Provided ThingDef</param>
		/// <returns>True for all meats.</returns>
		public static bool IsRegularMeat(in ThingDef def)
		{
			return def.IsIngestible && FoodUtility.GetMeatSourceCategory(def) == MeatSourceCategory.Undefined;
		}

		/// <summary>
		/// Returns true if the provided def is a stuff of the woody category.
		/// </summary>
		/// <param name="def">Provided ThingDef</param>
		/// <returns>True if the stuff is a type of wood.</returns>
		public static bool IsWoodyStuff(in ThingDef def)
		{
			return def.stuffProps?.categories != null && def.stuffProps.categories.Contains(StuffCategoryDefOf.Woody);
		}
	}
}