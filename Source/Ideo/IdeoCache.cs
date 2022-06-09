using System.Collections.Generic;
using System.Linq;
using Force.DeepCloner;
using RimWorld;
using TG.DefOf;
using TG.StockGen;
using Verse;

namespace TG.Ideo
{
	/// <summary>
	/// Caches information generated by PreceptGenExtension for each ideology.
	/// Ideo.id is used as the cache key.
	/// </summary>
	public static class IdeoStockCache
	{
		/// <summary>
		/// Stock generators added to visitors following this ideology.
		/// </summary>
		private static readonly Dictionary<int, List<StockGenerator>> _visitorStockGens =
			new Dictionary<int, List<StockGenerator>>();

		/// <summary>
		/// Stock generators added to caravans and orbital traders following this ideology.
		/// </summary>
		private static readonly Dictionary<int, List<StockGenerator>> _traderStockGens =
			new Dictionary<int, List<StockGenerator>>();

		/// <summary>
		/// Stock generators added to settlements of this ideology.
		/// </summary>
		private static readonly Dictionary<int, List<StockGenerator>> _settlementStockGens =
			new Dictionary<int, List<StockGenerator>>();

		/// <summary>
		/// Traders following this ideology refuse to trade these items.
		/// </summary>
		private static readonly Dictionary<int, HashSet<ThingDef>> _willNotTrade = new Dictionary<int, HashSet<ThingDef>>();

		/// <summary>
		/// Traders following this ideology never have these items in stock.
		/// </summary>
		private static readonly Dictionary<int, HashSet<ThingDef>> _willNotStock = new Dictionary<int, HashSet<ThingDef>>();

		/// <summary>
		/// Generates a PreceptGen for venerated animals, if required.
		/// </summary>
		/// <param name="ideo">Ideology being processed.</param>
		/// <returns>Null if the ideology has no venerated animals, a generated PreceptGen otherwise.</returns>
		private static PreceptGenDef VeneratedAnimalsPreceptGen(RimWorld.Ideo ideo)
		{
			PreceptGenDef veneratedPrecept = null;

			var obtainableVenerated = ideo.VeneratedAnimals.Select(thingDef => thingDef.race.AnyPawnKind)
				.Where(def => Things.Util.ObtainableAnimal(def)).ToList();

			if (obtainableVenerated.Count > 0)
			{
				veneratedPrecept = PreceptGen.TG_AutomaticVeneratedAnimal.ShallowClone();
				if (veneratedPrecept.visitorStockGens.Count >= 1 &&
				    veneratedPrecept.visitorStockGens[0] is VeneratedAnimals visitorAnimals &&
				    veneratedPrecept.traderStockGens.Count >= 1 &&
				    veneratedPrecept.traderStockGens[0] is VeneratedAnimals traderAnimals &&
				    veneratedPrecept.settlementStockGens.Count >= 1 &&
				    veneratedPrecept.settlementStockGens[0] is VeneratedAnimals settlementAnimals)
				{
					Logger.Gen(
						$"Adding venerated animal stock generator for these animals: {string.Join(", ", obtainableVenerated.Select(def => def.label))}.");
					visitorAnimals.pawnKindDefs = obtainableVenerated;
					traderAnimals.pawnKindDefs = obtainableVenerated;
					settlementAnimals.pawnKindDefs = obtainableVenerated;
				}
				else
				{
					Logger.ErrorOnce("TG_AutomaticVeneratedAnimal is not defined correctly.");
				}
			}

			return veneratedPrecept;
		}

		/// <summary>
		/// Gathers all PreceptGen definitions for this ideology.
		/// </summary>
		/// <param name="ideo">Ideology being processed.</param>
		/// <returns>List of PreceptGen definitions to apply.</returns>
		private static List<PreceptGenDef> GatherPreceptGens(RimWorld.Ideo ideo)
		{
			// Generic flags which may be added by any precept.
			var approvesOfCharity = false;
			var approvesOfSlavery = false;
			var likesHumanLeatherApparel = false;
			var willNotStockRawVegetables = false;
			var willNotStockRegularMeat = false;
			var willNotStockWood = false;

			var preceptGenDefs = new List<PreceptGenDef>();

			foreach (var precept in ideo.PreceptsListForReading)
			{
				approvesOfCharity = approvesOfCharity || precept.def.approvesOfCharity;
				approvesOfSlavery = approvesOfSlavery || precept.def.approvesOfSlavery;
				likesHumanLeatherApparel = likesHumanLeatherApparel || precept.def.likesHumanLeatherApparel;
				willNotStockRawVegetables = willNotStockRawVegetables || precept.def.disallowFarmingCamps;
				willNotStockRegularMeat = willNotStockRegularMeat || precept.def.disallowHuntingCamps;
				willNotStockWood = willNotStockWood || precept.def.disallowLoggingCamps;
			}

			// Automatically adds some PreceptGens based on the precepts checked before.
			if (approvesOfCharity)
			{
				preceptGenDefs.Add(PreceptGen.TG_AutomaticApprovesOfCharity);
			}

			if (approvesOfSlavery)
			{
				preceptGenDefs.Add(PreceptGen.TG_AutomaticApprovesOfSlavery);
			}

			preceptGenDefs.Add(likesHumanLeatherApparel
				? PreceptGen.TG_AutomaticLikesHumanApparel
				: PreceptGen.TG_AutomaticDislikesHumanApparel);

			if (willNotStockRawVegetables)
			{
				preceptGenDefs.Add(PreceptGen.TG_AutomaticNoRawVegan);
			}

			if (willNotStockRegularMeat)
			{
				preceptGenDefs.Add(PreceptGen.TG_AutomaticNoRegularMeat);
			}

			if (willNotStockWood)
			{
				preceptGenDefs.Add(PreceptGen.TG_AutomaticNoWoodyStock);
			}

			var veneratedGenDef = VeneratedAnimalsPreceptGen(ideo);
			if (veneratedGenDef != null)
			{
				preceptGenDefs.Add(veneratedGenDef);
			}

			return preceptGenDefs;
		}

		/// <summary>
		/// Precept stock generators and rules are processed here.
		/// </summary>
		/// <param name="ideo">Ideology being evaluated</param>
		/// <param name="preceptGenDefs">List of PreceptGen definitions for this ideo.</param>
		private static void EvaluatePreceptGens(RimWorld.Ideo ideo, List<PreceptGenDef> preceptGenDefs)
		{
			var key = ideo.id;

			var rules = new List<StockRule.Rule>();
			foreach (var preceptGenDef in preceptGenDefs)
			{
				_visitorStockGens[key].AddRange(preceptGenDef.visitorStockGens);
				_traderStockGens[key].AddRange(preceptGenDef.traderStockGens);
				_settlementStockGens[key].AddRange(preceptGenDef.settlementStockGens);
				rules.AddRange(preceptGenDef.stockRules);
			}

			foreach (var def in DefDatabase<ThingDef>.AllDefs)
			{
				foreach (var rule in rules)
				{
					if (rule.ForbidsTrading(def))
					{
						_willNotTrade[key].Add(def);
					}

					if (rule.ForbidsStocking(def))
					{
						_willNotStock[key].Add(def);
					}
				}
			}
		}


		/// <summary>
		/// Traders following an ideology will refuse to trade or stock the meat of their venerated animals.
		/// </summary>
		/// <param name="ideo"></param>
		private static void AddForbiddenMeats(RimWorld.Ideo ideo)
		{
			var forbiddenMeats = new List<ThingDef>();
			foreach (var veneratedDef in ideo.VeneratedAnimals.Where(veneratedDef =>
				         veneratedDef.race != null))
			{
				// Most animals use the meat type defined in their race. Some modded animals may just have placeholder
				// and a meat amount value of zero.
				if (veneratedDef.GetStatValueAbstract(StatDefOf.MeatAmount) > 0 && veneratedDef.race.meatDef != null &&
				    veneratedDef.race.meatDef.IsIngestible)
				{
					forbiddenMeats.Add(veneratedDef.race.meatDef);
				}

				// Some mods use butcherProducts instead.
				if (!veneratedDef.butcherProducts.NullOrEmpty())
				{
					forbiddenMeats.AddRange(veneratedDef.butcherProducts.Select(defCount => defCount.thingDef)
						.Where(def => def.IsIngestible));
				}
			}

			if (forbiddenMeats.NullOrEmpty()) return;
			Logger.Gen(
				$"Forbidding trading and stocking venerated animal meats: {string.Join(", ", forbiddenMeats.Select(def => def.label))}.");
			_willNotTrade[ideo.id].UnionWith(forbiddenMeats);
			_willNotStock[ideo.id].UnionWith(forbiddenMeats);
		}

		public static void Invalidate(RimWorld.Ideo ideo)
		{
			var key = ideo.id;
			_visitorStockGens.Remove(key);
			_traderStockGens.Remove(key);
			_settlementStockGens.Remove(key);
			_willNotTrade.Remove(key);
			_willNotStock.Remove(key);
		}

		/// <summary>
		/// Adds an ideology into the stock cache. Will not do anything if it already contains info.
		/// </summary>
		/// <param name="ideo">Ideology to add.</param>
		public static void TryAdd(RimWorld.Ideo ideo)
		{
			if (!ModsConfig.IdeologyActive || ideo == null || _traderStockGens.ContainsKey(ideo.id))
			{
				return;
			}

			Logger.Gen($"Generating ideology trading information for {ideo.name}.");

			var key = ideo.id;

			_visitorStockGens[key] = new List<StockGenerator>();
			_traderStockGens[key] = new List<StockGenerator>();
			_settlementStockGens[key] = new List<StockGenerator>();
			_willNotTrade[key] = new HashSet<ThingDef>();
			_willNotStock[key] = new HashSet<ThingDef>();

			var preceptGenDefs = GatherPreceptGens(ideo);
			EvaluatePreceptGens(ideo, preceptGenDefs);
			AddForbiddenMeats(ideo);
		}

		/// <summary>
		/// Returns StockGenerators to add to visitors following this ideology.
		/// </summary>
		/// <param name="ideo">Ideology to check.</param>
		/// <returns>Extra StockGenerators.</returns>
		public static List<StockGenerator> VisitorStockGens(RimWorld.Ideo ideo)
		{
			return _visitorStockGens.ContainsKey(ideo.id) ? _visitorStockGens[ideo.id] : null;
		}

		/// <summary>
		/// Returns StockGenerators to add to caravans and orbital traders following this ideology.
		/// </summary>
		/// <param name="ideo">Ideology to check.</param>
		/// <returns>Extra StockGenerators.</returns>
		public static List<StockGenerator> TraderStockGens(RimWorld.Ideo ideo)
		{
			return _traderStockGens.ContainsKey(ideo.id) ? _traderStockGens[ideo.id] : null;
		}

		/// <summary>
		/// Returns StockGenerators to add to settlements following this ideology.
		/// </summary>
		/// <param name="ideo">Ideology to check.</param>
		/// <returns>Extra StockGenerators.</returns>
		public static List<StockGenerator> SettlementStockGens(RimWorld.Ideo ideo)
		{
			return _settlementStockGens.ContainsKey(ideo.id) ? _settlementStockGens[ideo.id] : null;
		}

		/// <summary>
		/// Traders following this ideology refuse to trade these items.
		/// </summary>
		/// <param name="ideo">Ideology to check.</param>
		/// <returns>Items that will never be trade.</returns>
		public static HashSet<ThingDef> WillNotTrade(RimWorld.Ideo ideo)
		{
			return _willNotTrade.ContainsKey(ideo.id) ? _willNotTrade[ideo.id] : null;
		}

		/// <summary>
		/// Traders following this ideology refuse to stock these items.
		/// </summary>
		/// <param name="ideo">Ideology to check.</param>
		/// <returns>Items that will never be in stock.</returns>
		public static HashSet<ThingDef> WillNotStock(RimWorld.Ideo ideo)
		{
			return _willNotStock.ContainsKey(ideo.id) ? _willNotStock[ideo.id] : null;
		}
	}
}