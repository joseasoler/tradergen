using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
				b.Append($"thingDefs: {gen.thingDefs}\n");
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

			if (type == typeof(StockGenerator_WeaponsMelee))
			{
				var gen = (StockGenerator_WeaponsMelee) g;
				b.Append($"weaponTag: {gen.weaponTag}\n");
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

			return b;
		}
	}
}