using System.Collections.Generic;
using RimWorld;
using TG.DefOf;
using Verse;

namespace TG.Ideo
{
	/// <summary>
	/// Caches information generated by PreceptGenExtension for each ideology.
	/// Ideo.id is used as the cache key.
	/// </summary>
	public class IdeoStockCache : GameComponent
	{
		/// <summary>
		/// Stock generators added to visitors following this ideology.
		/// </summary>
		private readonly Dictionary<int, List<StockGenerator>> _visitorStockGens =
			new Dictionary<int, List<StockGenerator>>();

		/// <summary>
		/// Stock generators added to caravans and orbital traders following this ideology.
		/// </summary>
		private readonly Dictionary<int, List<StockGenerator>> _traderStockGens =
			new Dictionary<int, List<StockGenerator>>();

		/// <summary>
		/// Stock generators added to settlements of this ideology.
		/// </summary>
		private readonly Dictionary<int, List<StockGenerator>> _settlementStockGens =
			new Dictionary<int, List<StockGenerator>>();

		/// <summary>
		/// Traders following this ideology refuse to purchase these items.
		/// </summary>
		private readonly Dictionary<int, HashSet<ThingDef>> _willNotPurchase = new Dictionary<int, HashSet<ThingDef>>();

		/// <summary>
		/// Traders following this ideology never have these items in stock.
		/// </summary>
		private readonly Dictionary<int, HashSet<ThingDef>> _willNotStock = new Dictionary<int, HashSet<ThingDef>>();

		/// <summary>
		/// Caches the IdeoStockCache to avoid Current.Game.GetComponent calls.
		/// </summary>
		public static IdeoStockCache Instance;

		public IdeoStockCache()
		{
			Instance = this;
		}

		public IdeoStockCache(Game game)
		{
			Instance = this;
		}

		private static List<PreceptGenDef> GatherPreceptGens(RimWorld.Ideo ideo)
		{
			// Generic flags which may be added in any precept.
			var approvesOfCharity = false;
			var approvesOfSlavery = false;
			var likesHumanLeatherApparel = false;
			var willNotStockRawVegetables = false;

			var preceptGenDefs = new List<PreceptGenDef>();

			foreach (var precept in ideo.PreceptsListForReading)
			{
				approvesOfCharity = approvesOfCharity || precept.def.approvesOfCharity;
				approvesOfSlavery = approvesOfSlavery || precept.def.approvesOfSlavery;
				likesHumanLeatherApparel = likesHumanLeatherApparel || precept.def.likesHumanLeatherApparel;
				willNotStockRawVegetables = willNotStockRawVegetables || precept.def.disallowFarmingCamps;
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
				preceptGenDefs.Add(PreceptGen.TG_AutomaticNoRawVegetables);
			}

			return preceptGenDefs;
		}

		/// <summary>
		/// Precept rules are evaluated only once and their results are cached here.
		/// </summary>
		/// <param name="ideo">Ideology being evaluated</param>
		/// <param name="preceptGenDefs">List of precept gen definitions for this ideo.</param>
		private void EvaluatePreceptGens(RimWorld.Ideo ideo, List<PreceptGenDef> preceptGenDefs)
		{
			var key = ideo.id;

			var rules = new List<Rule.Rule>();
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
					if (rule.ForbidsPurchase(def))
					{
						_willNotPurchase[key].Add(def);
					}

					if (rule.ForbidsStocking(def))
					{
						_willNotStock[key].Add(def);
					}
				}
			}
		}

		public void Invalidate(RimWorld.Ideo ideo)
		{
			var key = ideo.id;
			_visitorStockGens.Remove(key);
			_traderStockGens.Remove(key);
			_settlementStockGens.Remove(key);
			_willNotPurchase.Remove(key);
			_willNotStock.Remove(key);
		}

		/// <summary>
		/// Adds an ideology into the stock cache. Will not do anything if it already contains info.
		/// </summary>
		/// <param name="ideo">Ideology to add.</param>
		public void TryAdd(RimWorld.Ideo ideo)
		{
			if (!ModsConfig.IdeologyActive || ideo == null || _traderStockGens.ContainsKey(ideo.id))
			{
				return;
			}

			var key = ideo.id;

			_visitorStockGens[key] = new List<StockGenerator>();
			_traderStockGens[key] = new List<StockGenerator>();
			_settlementStockGens[key] = new List<StockGenerator>();
			_willNotPurchase[key] = new HashSet<ThingDef>();
			_willNotStock[key] = new HashSet<ThingDef>();

			var preceptGenDefs = GatherPreceptGens(ideo);
			EvaluatePreceptGens(ideo, preceptGenDefs);
		}

		/// <summary>
		/// Returns StockGenerators to add to visitors following this ideology.
		/// </summary>
		/// <param name="ideo">Ideology to check.</param>
		/// <returns>Extra StockGenerators.</returns>
		public List<StockGenerator> VisitorStockGens(RimWorld.Ideo ideo)
		{
			return _visitorStockGens.ContainsKey(ideo.id) ? _visitorStockGens[ideo.id] : new List<StockGenerator>();
		}

		/// <summary>
		/// Returns StockGenerators to add to caravans and orbital traders following this ideology.
		/// </summary>
		/// <param name="ideo">Ideology to check.</param>
		/// <returns>Extra StockGenerators.</returns>
		public List<StockGenerator> TraderStockGens(RimWorld.Ideo ideo)
		{
			return _traderStockGens.ContainsKey(ideo.id) ? _traderStockGens[ideo.id] : new List<StockGenerator>();
		}

		/// <summary>
		/// Returns StockGenerators to add to settlements following this ideology.
		/// </summary>
		/// <param name="ideo">Ideology to check.</param>
		/// <returns>Extra StockGenerators.</returns>
		public List<StockGenerator> SettlementStockGens(RimWorld.Ideo ideo)
		{
			return _settlementStockGens.ContainsKey(ideo.id) ? _settlementStockGens[ideo.id] : new List<StockGenerator>();
		}

		/// <summary>
		/// Checks if an ideology allows purchasing a specific item.
		/// </summary>
		/// <param name="ideoId">Id of the ideology to check.</param>
		/// <param name="def">Item to check</param>
		/// <returns>True if the item can be purchased.</returns>
		public bool Purchases(int ideoId, ThingDef def)
		{
			return !_willNotPurchase.ContainsKey(ideoId) || !_willNotPurchase[ideoId].Contains(def);
		}

		/// <summary>
		/// Checks if an ideology allows having a specific item in stock.
		/// </summary>
		/// <param name="ideoId">Id of the ideology to check.</param>
		/// <param name="def">Item to check</param>
		/// <returns>True if the item can be in stock.</returns>
		public bool Stocks(int ideoId, ThingDef def)
		{
			return !_willNotPurchase.ContainsKey(ideoId) || !_willNotStock[ideoId].Contains(def);
		}
	}
}