using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using TG.TraderKind;
using UnityEngine;
using Verse;

namespace TG.Settlement
{
	/// <summary>
	/// Returns the TraderKindDef to use for settlements.
	/// Settlements are a special case because they do not hold a specific reference to a TraderKindDef that could be
	/// patched. Instead, Settlement_TraderTracker.TraderKind calculates one among the available baseTraderKinds of the
	/// faction.
	/// </summary>
	public class SettlementTraderData : WorldComponent
	{
		// Stores generated traderKindDefs for each settlement.
		private Dictionary<RimWorld.Planet.Settlement, TraderKindDef> _cache =
			new Dictionary<RimWorld.Planet.Settlement, TraderKindDef>();

		// Stores the value of NextRestockTick when the traderKindDef was generated last time.
		private Dictionary<RimWorld.Planet.Settlement, int> _nextRestockTick =
			new Dictionary<RimWorld.Planet.Settlement, int>();

		
		public SettlementTraderData(World world) : base(world)
		{
		}

		/// <summary>
		/// Obtain the TraderKindDef to use as a template when generating TraderKindDefs for a settlement.
		/// Mirrors the vanilla implementation of Settlement_TraderTracker.TraderKind.
		/// </summary>
		/// <param name="settlement">Settlement trader being generated.</param>
		/// <returns>Template TraderKindDef.</returns>
		private static TraderKindDef TemplateTraderKindDef(RimWorld.Planet.Settlement settlement)
		{
			var baseTraderKinds = settlement.Faction.def.baseTraderKinds;
			if (baseTraderKinds.NullOrEmpty())
			{
				return null;
			}

			var index = Mathf.Abs(settlement.HashOffset()) % baseTraderKinds.Count;
			return baseTraderKinds[index];
		}

		public TraderKindDef Get(RimWorld.Planet.Settlement settlement)
		{
			if (settlement.trader == null)
			{
				return null;
			}

			var nextRestockTick = settlement.NextRestockTick;
			if (!_cache.ContainsKey(settlement) || nextRestockTick > _nextRestockTick[settlement])
			{
				_nextRestockTick[settlement] = nextRestockTick;
				var traderKind = TemplateTraderKindDef(settlement);
				if (traderKind != null)
				{
					// RandomPriceFactorSeed is enough for other traders because they never restock.
					// Settlements need a different random seed for each restock, while keeping generation deterministic.
					var seed = Gen.HashCombineInt(settlement.trader.RandomPriceFactorSeed, nextRestockTick);
					traderKind = Generator.Def(traderKind, seed, settlement.Tile, settlement.Faction);
				}

				_cache[settlement] = traderKind;
			}

			return _cache[settlement];
		}
	}
}