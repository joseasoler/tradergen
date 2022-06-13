using System;
using System.Collections.Generic;
using System.Linq;
using Force.DeepCloner;
using RimWorld;
using RimWorld.Planet;
using TG.Ideo;
using TG.Mod;
using Verse;

namespace TG.TraderKind
{
	/// <summary>
	/// Maps a seed (RandomPriceFactorSeed of the trader) to additional procedurally generated trader information.
	/// Information is lazily initialized.
	/// Callers are responsible for clearing the cache.
	/// Nothing is saved or loaded.
	/// </summary>
	public static class Cache
	{
		/// <summary>
		/// Additional generators of the given seed.
		/// </summary>
		private static readonly Dictionary<int, List<StockGenerator>> _stockGens =
			new Dictionary<int, List<StockGenerator>>();

		/// <summary>
		/// Tradeability status of each item for this seed. This usually depends on StockGenerators, but also on extra
		/// TraderGen contents such as Ideology support.
		/// </summary>
		private static readonly Dictionary<int, Dictionary<ThingDef, bool>> _willTrade =
			new Dictionary<int, Dictionary<ThingDef, bool>>();

		/// <summary>
		/// This seed will never stock these items. This overrides default stockGenerator behavior.
		/// </summary>
		private static readonly Dictionary<int, HashSet<ThingDef>> _willNotStock = new Dictionary<int, HashSet<ThingDef>>();

		/// <summary>
		/// Specializations of this seed.
		/// </summary>
		private static readonly Dictionary<int, List<TraderSpecializationDef>> _specializations =
			new Dictionary<int, List<TraderSpecializationDef>>();

		private static int _generationSeed;

		/// <summary>
		/// RandomPriceFactorSeed is enough for traders which never restock.
		/// Settlements need a combined random seed which takes time into account for each restock.
		/// </summary>
		/// <param name="trader"></param>
		/// <returns></returns>
		private static int SeedFromTrader(ITrader trader)
		{
			if (trader is Settlement settlement)
			{
				return Gen.HashCombineInt(settlement.RandomPriceFactorSeed, settlement.trader.lastStockGenerationTicks);
			}

			return trader.RandomPriceFactorSeed;
		}

		public static int GenerationSeed => _generationSeed;

		/// <summary>
		/// Sets the seed used by the cache to generate TraderGen info. It should be set before any calls to TryAdd and
		/// any calls to stock generation.
		/// </summary>
		/// <param name="trader">Trader about to be generated.</param>
		public static void SetSeed(ITrader trader)
		{
			_generationSeed = SeedFromTrader(trader);
		}

		/// <summary>
		/// Sets the seed used by the cache to generate TraderGen info. It should be set before any calls to TryAdd and
		/// any calls to stock generation. Intended for Harmony patches which cannot obtain their ITrader.
		/// </summary>
		/// <param name="seed">Seed to use</param>
		public static void SetSeed(int seed)
		{
			_generationSeed = seed;
		}

		/// <summary>
		/// Provides all additional stock generators of a given seed.
		/// </summary>
		/// <param name="seed">Trader seed.</param>
		/// <returns>Extra stock generators.</returns>
		public static List<StockGenerator> StockGens(int seed)
		{
			return _stockGens.ContainsKey(seed) ? _stockGens[seed] : new List<StockGenerator>();
		}

		/// <summary>
		/// Checks if the trader is willing to trade a specific item.
		/// Results are lazily cached.
		/// </summary>
		/// <param name="trader">Trader.</param>
		/// <param name="traderDef">Trader definition.</param>
		/// <param name="thingDef">Thing being checked.</param>
		/// <returns>Boolean flag.</returns>
		public static bool WillTrade(ITrader trader, TraderKindDef traderDef, ThingDef thingDef)
		{
			var seed = SeedFromTrader(trader);
			if (!_willTrade.ContainsKey(seed))
			{
				Logger.ErrorOnce($"TraderGen cache is missing data for trader {traderDef.defName} {seed}");
				return false;
			}

			if (_willTrade[seed].TryGetValue(thingDef, out var willTradeThingDef)) return willTradeThingDef;

			willTradeThingDef = Enumerable.Any(traderDef.stockGenerators, stockGen => stockGen.HandlesThingDef(thingDef)) ||
			                    Enumerable.Any(_stockGens[seed], stockGen => stockGen.HandlesThingDef(thingDef));

			_willTrade[seed][thingDef] = willTradeThingDef;

			return willTradeThingDef;
		}

		/// <summary>
		/// Checks if the trader is forbidden from stocking a specific item.
		/// </summary>
		/// <param name="seed">Trader seed.</param>
		/// <param name="thingDef">Thing being checked.</param>
		/// <returns>Boolean flag.</returns>
		public static bool WillNotStock(int seed, ThingDef thingDef)
		{
			return _willNotStock.ContainsKey(seed) && _willNotStock[seed].Contains(thingDef);
		}

		/// <summary>
		/// Specializations of this trader.
		/// </summary>
		/// <param name="trader">Trader.</param>
		/// <returns>Custom label.</returns>
		public static List<TraderSpecializationDef> Specializations(ITrader trader)
		{
			var seed = SeedFromTrader(trader);
			return _specializations.ContainsKey(seed) ? _specializations[seed] : null;
		}

		/// <summary>
		/// Creates a copy with initialized internal state if necessary. Logs stock generator information if enabled.
		/// </summary>
		/// <param name="def">TraderKindDef associated with the stock generator.</param>
		/// <param name="gen">StockGenerator being added.</param>
		/// <param name="tile">Map tile in which the transaction takes place.</param>
		/// <param name="faction">Faction of the trader.</param>
		/// <returns></returns>
		private static StockGenerator StockGenFrom(TraderKindDef def, StockGenerator gen, int tile, Faction faction)
		{
			var generator = gen.ShallowClone();
			generator.ResolveReferences(def);

			if (gen.GetType().IsSubclassOf(typeof(TG.StockGen.StockGen)))
			{
				((TG.StockGen.StockGen) generator).BeforeGen(tile, faction);
			}

			return generator;
		}

		/// <summary>
		/// Apply specializations to the cache, if any.
		/// </summary>
		/// <param name="seed">Seed in use.</param>
		/// <param name="def">TraderKindDef in use.</param>
		/// <param name="tile">Map tile in which the transaction takes place.</param>
		/// <param name="faction">Faction of the trader.</param>
		private static void ApplySpecializations(int seed, TraderKindDef def, int tile, Faction faction)
		{
			var extension = def.GetModExtension<GenExtension>();
			if (extension == null) return;

			var numSpecializations = Math.Max(Settings.OrbitalSpecializations.RandomInRange, 0);
			if (numSpecializations <= 0) return;

			var chosenSpecializations =
				Algorithm.ChooseNWeightedRandomly(extension.specializations, spec => spec.commonality,
					numSpecializations);

			foreach (var specialization in chosenSpecializations)
			{
				Logger.Gen($"Adding specialization {specialization.def.defName}");
				_specializations[seed].Add(specialization.def);

				foreach (var gen in specialization.def.stockGens)
				{
					_stockGens[seed].Add(StockGenFrom(def, gen, tile, faction));
				}
			}
		}

		/// <summary>
		/// When the Ideology DLC is active and the trader follows a faction with an ideology, some stock changes dependent
		/// on precepts will be applied. Check IdeoStockCache for details.
		/// </summary>
		/// <param name="seed">Seed in use.</param>
		/// <param name="def">Trader</param>
		/// <param name="tile">Map tile in which the transaction takes place.</param>
		/// <param name="faction">Faction of the trader.</param>
		private static void ApplyIdeology(int seed, TraderKindDef def, int tile, Faction faction)
		{
			if (!ModsConfig.IdeologyActive) return;

			var ideo = faction?.ideos.PrimaryIdeo;
			if (ideo == null) return;

			Logger.Gen($"Applying ideology {ideo.name} of faction {faction.def.defName}");

			// The cache is lazily initialized here.
			IdeoStockCache.TryAdd(ideo);

			var willNotTradeList = IdeoStockCache.WillNotTrade(ideo);
			var willNotStockList = IdeoStockCache.WillNotStock(ideo);
			Logger.Gen(
				$"Adding {willNotTradeList.Count} items that will not be traded and {willNotStockList.Count} items that will not be generated.");

			foreach (var thingDef in IdeoStockCache.WillNotTrade(ideo))
			{
				_willTrade[seed][thingDef] = false;
			}

			_willNotStock[seed].UnionWith(IdeoStockCache.WillNotStock(ideo));

			List<StockGenerator> generatorsToAdd = null;

			var traderCategory = Util.GetTraderCategory(def, faction);
			switch (traderCategory)
			{
				case TraderKindCategory.Visitor:
					generatorsToAdd = IdeoStockCache.VisitorStockGens(ideo);
					break;
				case TraderKindCategory.Settlement:
					generatorsToAdd = IdeoStockCache.SettlementStockGens(ideo);
					break;
				case TraderKindCategory.Caravan:
				case TraderKindCategory.Orbital:
					generatorsToAdd = IdeoStockCache.TraderStockGens(ideo);
					break;
				case TraderKindCategory.None:
				default:
					break;
			}

			if (generatorsToAdd == null) return;

			Logger.Gen(
				$"Adding {generatorsToAdd.Count} ideology generators of the {Enum.GetName(typeof(TraderKindCategory), traderCategory)} category.");
			foreach (var gen in generatorsToAdd)
			{
				_stockGens[seed].Add(StockGenFrom(def, gen, tile, faction));
			}
		}

		/// <summary>
		/// Generate extra procedurally generated trader information.
		/// This process is deterministic; it will always produce the same result with the same template and seed.
		/// The caller is responsible for setting GenerationSeed before this function is called.
		/// StockGenerators with randomized internal state will be correctly initialized.
		/// Specializations will be added, if the TraderKindDef has a GenExtension defining them.
		/// If the Ideology DLC is present and the trader belongs to a faction, ideology precepts may add extra stock.
		/// </summary>
		/// <param name="def">Trader</param>
		/// <param name="tile">Map tile in which the transaction takes place.</param>
		/// <param name="faction">Faction of the trader.</param>
		public static void TryAdd(TraderKindDef def, int tile, Faction faction)
		{
			var seed = GenerationSeed;
			// The TraderGen system is not intended for favor traders.
			if (def.tradeCurrency == TradeCurrency.Favor || _stockGens.ContainsKey(seed)) return;

			// Initialize containers of this seed data.
			_stockGens[seed] = new List<StockGenerator>();
			_willTrade[seed] = new Dictionary<ThingDef, bool>();
			_willNotStock[seed] = new HashSet<ThingDef>();
			_specializations[seed] = new List<TraderSpecializationDef>();

			Logger.Gen($"Generating trader information for {def.defName} with seed {seed}.");
			Rand.PushState(seed);
			ApplySpecializations(seed, def, tile, faction);
			ApplyIdeology(seed, def, tile, faction);
			Rand.PopState();
		}

		/// <summary>
		/// Clears the cache of a specific trader.
		/// </summary>
		/// <param name="trader">Trader being cleared.</param>
		public static void Remove(ITrader trader)
		{
			var seed = SeedFromTrader(trader);
			Logger.Gen($"Clearing TraderGen cache of seed {seed}.");

			_stockGens.Remove(seed);
			_willTrade.Remove(seed);
			_willNotStock.Remove(seed);
			_specializations.Remove(seed);
		}

		/// <summary>
		/// Deterministic generation of a random name for an orbital trader with a GenExtension.
		/// Since this value is only used once, it is currently not being cached.
		/// Intended for debug action usage.
		/// </summary>
		/// <param name="seed">Seed in use.</param>
		/// <param name="def">Trader</param>
		/// <param name="faction">Faction of the trader.</param>
		/// <returns>Procedurally generated name.</returns>
		public static string Name(int seed, TraderKindDef def, Faction faction)
		{
			var extension = def.GetModExtension<GenExtension>();
			if (extension?.extraNameRules == null) return null;

			Rand.PushState(seed);
			var name = NameGenerator.GenerateName(extension.extraNameRules);
			Rand.PopState();

			if (faction != null)
			{
				name = $"{name} {"OfLower".Translate()} {faction.name}";
			}

			return name;
		}


		/// <summary>
		/// Deterministic generation of a random name for an orbital trader with a GenExtension.
		/// Since this value is only used once, it is currently not being cached.
		/// </summary>
		/// <param name="trader">Trader being named.</param>
		/// <returns>Procedurally generated name.</returns>
		public static string Name(ITrader trader)
		{
			return Name(SeedFromTrader(trader), trader.TraderKind, trader.Faction);
		}
	}
}