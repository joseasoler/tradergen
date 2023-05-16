using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace TraderGen.StockGen
{
	/// <summary>
	/// Chooses a ThingDef with stuffProps from a specific StuffCategory and generates stock made from it.
	/// </summary>
	public class RandomStuffFromCategory : FromStuff
	{
		/// <summary>
		/// A random material from this category will be chosen.
		/// </summary>
		public StuffCategoryDef stuffCategoryDef;

		/// <summary>
		/// Never choose any of these stuff defs.
		/// </summary>
		public List<ThingDef> excludeStuffDefs = new List<ThingDef>();

		/// <summary>
		/// Allow choosing stuff which should never be sold by traders. Keep in mind that the things made from this stuff
		/// still need to be tradeable.
		/// </summary>
		public bool allowNonTradeableStuff = false;

		public override void ConditionToText(ref StringBuilder b)
		{
			base.ConditionToText(ref b);
			b.Append($"stuffCategoryDef: {stuffCategoryDef}\n");
			Util.ToText(ref b, "excludeStuffDefs", excludeStuffDefs);
			if (allowNonTradeableStuff)
			{
				b.Append("allowNonTradeableStuff: true\n");
			}
		}

		/// <summary>
		/// Materials belonging to explicit-only stuffCategories are only used if their stuffCategory is selected
		/// explicitly. This prevents stuffs having two stuffCategories from being too frequent.
		/// </summary>
		private static readonly List<string> ExplicitOnlyStuffCategories = new List<string>
		{
			"BMT_ChitinStuff", // Chitin from Biomes!
			"Gemstones", // Gemstones from Jewelry
			"RB_Waxy" // Wax from Alpha Bees
		};

		/// <summary>
		/// Choose a material with the specified category matching certain criteria.
		/// </summary>
		protected override void SetStuffDef()
		{
			var stuffDefs = DefDatabase<ThingDef>.AllDefs.Where(stuffDef =>
				// Stuff belonging to the chosen category.
				stuffDef.stuffProps?.categories != null && stuffDef.stuffProps.categories.Contains(stuffCategoryDef) &&
				// Stuff which is not excluded from being used.
				!excludeStuffDefs.Contains(stuffDef) &&
				// Avoid materials not intended for sale.
				(allowNonTradeableStuff || stuffDef.stuffProps.commonality > 0.0f && stuffDef.tradeability.TraderCanSell()) &&
				stuffDef.PlayerAcquirable &&
				// Avoid hyperweave and modded materials such as archotech mass.
				(stuffDef.tradeTags == null || !stuffDef.tradeTags.Contains("ExoticMisc")) &&
				// Materials belonging to explicit-only stuffCategories are only used if their stuffCategory is selected
				// explicitly. This prevents stuffs having two stuffCategories from being too frequent.
				ExplicitOnlyStuffCategories.All(cat =>
					stuffCategoryDef.defName == cat || !stuffDef.stuffProps.categories.Any(c => c.defName == cat)) &&
				// VFEArch_WoodLog types rely on VFECore.StuffExtension to set its commonality to zero. To avoid a dependency on
				// VFE or adding complex reflection code, they are hard-coded to be removed here instead.
				!stuffDef.defName.StartsWith("VFEArch_WoodLog_")
			);

			if (stuffDefs.TryRandomElementByWeight(Util.RandomStuffDefWeight, out _stuffDef))
			{
				return;
			}

			var defName = stuffCategoryDef != null ? stuffCategoryDef.defName : "null";
			Logger.ErrorOnce($"TraderGen.StockGen.FromStuff: Could not find random stuffDef from category {defName}");
			_stuffDef = ThingDefOf.Steel;
		}
	}
}