using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using TG.DefOf;
using Verse;

namespace TG.StockGen
{
	/// <summary>
	/// Utility functions used by different StockGens.
	/// </summary>
	public static class Util
	{
		/// <summary>
		/// Material random weight from market value.
		/// </summary>
		private static readonly SimpleCurve RandomStuffMarketValueWeight = new SimpleCurve
		{
			new CurvePoint(2f, 1f),
			new CurvePoint(3f, 0.8f),
			new CurvePoint(5f, 0.3f),
			new CurvePoint(8, 0.2f),
			new CurvePoint(10, 0.05f)
		};

		/// <summary>
		/// Material random weight from mass.
		/// </summary>
		private static readonly SimpleCurve RandomStuffMassWeight = new SimpleCurve
		{
			new CurvePoint(0.8f, 0.2f),
			new CurvePoint(0.5f, 1f),
			new CurvePoint(0.4f, 0.6f),
			new CurvePoint(0.2f, 0.4f),
			new CurvePoint(0.1f, 0.2f),
			new CurvePoint(0.01f, 0.1f)
		};

		/// <summary>
		/// Weight used for choosing randomly between different materials. Prefers cheap and light stuff.
		/// </summary>
		/// <param name="stuffDef">ThingDef assumed to have valid stuffProps.</param>
		/// <returns>Stuff weight for the choosing algorithm.</returns>
		public static float RandomStuffDefWeight(ThingDef stuffDef)
		{
			return stuffDef.stuffProps.commonality * (RandomStuffMarketValueWeight.Evaluate(stuffDef.BaseMarketValue) +
			                                          RandomStuffMassWeight.Evaluate(stuffDef.BaseMass) +
			                                          (!stuffDef.smallVolume ? 0.4f : 0.0f));
		}

		public static void ToText<T>(ref StringBuilder b, string label, IReadOnlyCollection<T> list)
		{
			if (list != null && list.Count > 0)
			{
				b.Append($"{label}: {string.Join(", ", list)}\n");
			}
		}

		private static void DerivedToText(ref StringBuilder b, StockGenerator g)
		{
			var type = g.GetType();

			if (type.IsSubclassOf(typeof(StockGen)))
			{
				var gen = (StockGen) g;
				gen.ToText(ref b);
				return;
			}

			if (type == typeof(StockGenerator_Animals))
			{
				var gen = (StockGenerator_Animals) g;
				ToText(ref b, "tradeTagsBuy", gen.tradeTagsBuy);
				ToText(ref b, "tradeTagsSell", gen.tradeTagsSell);

				if (gen.kindCountRange != IntRange.one)
				{
					b.Append($"kindCountRange: {gen.kindCountRange}\n");
				}

				b.Append($"wildness: {gen.minWildness}~{gen.maxWildness}\n");
				b.Append($"checkTemperature: {gen.checkTemperature}\n");

				return;
			}

			if (type == typeof(StockGenerator_BuySingleDef))
			{
				var gen = (StockGenerator_BuySingleDef) g;
				b.Append($"thingDef: {gen.thingDef}\n");
				return;
			}

			if (type == typeof(StockGenerator_Category))
			{
				var gen = (StockGenerator_Category) g;
				b.Append($"categoryDef: {gen.categoryDef}\n");
				if (gen.thingDefCountRange != IntRange.one)
				{
					b.Append($"thingDefCountRange: {gen.thingDefCountRange}\n");
				}

				ToText(ref b, "excludedThingDefs", gen.excludedThingDefs);
				ToText(ref b, "excludedCategories", gen.excludedCategories);

				return;
			}

			if (type == typeof(StockGenerator_MultiDef))
			{
				var gen = (StockGenerator_MultiDef) g;
				ToText(ref b, "thingDefs", gen.thingDefs);
				return;
			}

			if (type == typeof(StockGenerator_SingleDef))
			{
				var gen = (StockGenerator_SingleDef) g;
				b.Append($"thingDef: {gen.thingDef}\n");
				return;
			}

			if (type == typeof(StockGenerator_Tag))
			{
				var gen = (StockGenerator_Tag) g;
				b.Append($"tradeTag: {gen.tradeTag}");
				b.Append($"thingDefCountRange: {gen.thingDefCountRange}\n");
				ToText(ref b, "excludedThingDefs", gen.excludedThingDefs);

				return;
			}

			if (type == typeof(StockGenerator_Techprints))
			{
				var gen = (StockGenerator_Techprints) g;
				ToText(ref b, "countChances", gen.countChances);


				return;
			}

			if (type == typeof(StockGenerator_BuyTradeTag))
			{
				var gen = (StockGenerator_BuyTradeTag) g;
				b.Append($"tag: {gen.tag}\n");
				return;
			}

			if (type == typeof(StockGenerator_MarketValue))
			{
				var gen = (StockGenerator_MarketValue) g;
				b.Append($"tradeTag: {gen.tradeTag}\n");
				b.Append($"weaponTag: {gen.weaponTag}\n");
				b.Append($"apparelTag: {gen.apparelTag}\n");
				return;
			}

			if (type == typeof(StockGenerator_Slaves))
			{
				var gen = (StockGenerator_Slaves) g;
				b.Append($"respectPopulationIntent: {gen.respectPopulationIntent}\n");
				if (gen.slaveKindDef != null)
				{
					b.Append($"slaveKindDef: {gen.slaveKindDef.race.defName}\n");
				}

				return;
			}
		}

		public static StringBuilder ToText(StockGenerator g)
		{
			var b = new StringBuilder();
			b.Append($"{g.GetType().Name}:\n");

			DerivedToText(ref b, g);

			if (g.countRange != IntRange.zero)
			{
				b.Append($"countRange: {g.countRange}\n");
			}

			ToText(ref b, "customCountRanges", g.customCountRanges);

			if (g.totalPriceRange != FloatRange.Zero)
			{
				b.Append($"totalPriceRange: {g.totalPriceRange}\n");
			}

			if (g.maxTechLevelGenerate != TechLevel.Archotech)
			{
				b.Append($"maxTechLevelGenerate: {Enum.GetName(typeof(TechLevel), g.maxTechLevelGenerate)}\n");
			}

			if (g.maxTechLevelBuy != TechLevel.Archotech)
			{
				b.Append($"maxTechLevelBuy: {Enum.GetName(typeof(TechLevel), g.maxTechLevelBuy)}\n");
			}

			if (g.price != PriceType.Normal)
			{
				b.Append($"price: {Enum.GetName(typeof(PriceType), g.price)}\n");
			}

			// Remove the last end of line character.
			if (b.Length > 0 && b[b.Length - 1] == '\n') b.Remove(b.Length - 1, 1);

			return b;
		}
	}
}