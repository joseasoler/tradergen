using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace TG.StockGen
{
	/// <summary>
	/// Chooses a ThingDef with stuffProps from a specific StuffCategory and generates stock made from it.
	/// </summary>
	public class FromStuffCategory : FromStuff
	{
		public StuffCategoryDef stuffCategoryDef;

		public List<ThingDef> excludeThingDefs;

		/// <summary>
		/// Materials belonging to explicit-only stuffCategories are only used if their stuffCategory is selected
		/// explicitly. This prevents stuffs having two stuffCategories from being too frequent.
		/// </summary>
		private static readonly List<string> ExplicitOnlyStuffCategories = new List<string>
		{
			"RB_Waxy",
			"Gemstones"
		};

		/// <summary>
		/// Material random weight from market value.
		/// </summary>
		private static readonly SimpleCurve MarketValueWeight = new SimpleCurve
		{
			new CurvePoint(2f, 1f),
			new CurvePoint(3f, 0.8f),
			new CurvePoint(5f, 0.4f),
			new CurvePoint(8, 0.3f),
			new CurvePoint(10, 0.1f)
		};

		/// <summary>
		/// Material random weight from mass.
		/// </summary>
		private static readonly SimpleCurve MassWeight = new SimpleCurve
		{
			new CurvePoint(0.8f, 0.2f),
			new CurvePoint(0.5f, 1f),
			new CurvePoint(0.4f, 0.6f),
			new CurvePoint(0.2f, 0.4f),
			new CurvePoint(0.1f, 0.2f),
			new CurvePoint(0.01f, 0.1f)
		};

		/// <summary>
		/// Weight used for randomly choosing a material to use.
		/// </summary>
		/// <param name="stuffDef">ThingDef being considered as material.</param>
		/// <returns>Total weight for the random choosing algorithm.</returns>
		private static float StuffDefWeight(ThingDef stuffDef)
		{
			return 3.0f * stuffDef.stuffProps.commonality + MarketValueWeight.Evaluate(stuffDef.BaseMarketValue) +
			       MassWeight.Evaluate(stuffDef.BaseMass) + (!stuffDef.smallVolume ? 0.4f : 0.0f);
		}

		/// <summary>
		/// Choose a material with the specified category matching certain criteria.
		/// </summary>
		protected override void SetStuffDef()
		{
			var stuffDefs = DefDatabase<ThingDef>.AllDefs.Where(stuffDef =>
				// Stuff belonging to the chosen category.
				stuffDef.stuffProps?.categories != null && stuffDef.stuffProps.categories.Contains(stuffCategoryDef) &&
				// Stuff which is not excluded from being used.
				(excludeThingDefs == null || !excludeThingDefs.Contains(stuffDef)) &&
				// Avoid materials not intended for sale.
				stuffDef.stuffProps.commonality > 0.0f && stuffDef.tradeability.TraderCanSell() && stuffDef.PlayerAcquirable &&
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

			if (stuffDefs.TryRandomElementByWeight(StuffDefWeight, out _stuffDef))
			{
				return;
			}

			var defName = stuffCategoryDef != null ? stuffCategoryDef.defName : "null";
			Logger.ErrorOnce($"TG.StockGen.FromStuff: Could not find random stuffDef from category {defName}");
			_stuffDef = ThingDefOf.Steel;
		}
	}
}