using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using TG.Mod;
using TraderGen.Mod;
using TraderGen.TraderKind;
using Verse;

namespace TraderGen.Harmony
{
	/// <summary>
	/// Prepares extra TraderGen information before vanilla generation takes place.
	/// Then generates extra TraderGen content after vanilla generation.
	/// </summary>
	[HarmonyPatch]
	public static class TraderStock
	{
		/// <summary>
		/// Prepares the extra TraderGen information. This must be done before vanilla content is generated as TraderGen
		/// may need to forbid some of the stock they create.
		/// </summary>
		/// <param name="parms">Group of parameters to be used for stock generation.</param>
		/// <param name="outThings">List of generated things.</param>
		[HarmonyPrefix]
		[HarmonyPatch(typeof(ThingSetMaker_TraderStock), nameof(ThingSetMaker_TraderStock.Generate))]
		private static void GeneratePrefix(ThingSetMakerParams parms, ref List<Thing> outThings)
		{
			var trader = parms.traderDef;
			var tile = parms.tile ?? -1;
			var faction = parms.makingFaction;
			// Add trader to the cache. The seed must have been set beforehand by patches in TradeShip, Pawn or Settlement.
			Cache.TryAdd(trader, tile, faction);
		}

		/// <summary>
		/// Generates extra TraderGen stock after generating the vanilla stock.
		/// Logs a generated things report if requested.
		/// </summary>
		/// <param name="parms">Group of parameters to be used for stock generation.</param>
		/// <param name="outThings">List of generated things.</param>
		[HarmonyPostfix]
		[HarmonyPatch(typeof(ThingSetMaker_TraderStock), nameof(ThingSetMaker_TraderStock.Generate))]
		private static void GeneratePostfix(ThingSetMakerParams parms, ref List<Thing> outThings)
		{
			var trader = parms.traderDef;
			var tile = parms.tile ?? -1;
			var faction = parms.makingFaction;

			// Include stock generator information from the original trader.
			if (Settings.LogGen && Settings.LogStockGen)
			{
				foreach (var stockGen in trader.stockGenerators)
				{
					Logger.Gen(StockGen.Util.ToText(stockGen).ToString());
				}
			}

			foreach (var stockGen in Cache.StockGens(Cache.GenerationSeed))
			{
				foreach (var thing in stockGen.GenerateThings(tile, faction))
				{
					if (!thing.def.tradeability.TraderCanSell())
					{
						Log.Error(
							$"{trader.defName} generated carrying {thing.def.defName} which can't be sold by traders. Ignoring...");
					}
					else
					{
						thing.PostGeneratedForTrader(parms.traderDef, tile, faction);
						outThings.Add(thing);
					}

					if (Settings.LogGen && Settings.LogStockGen)
					{
						Logger.Gen(StockGen.Util.ToText(stockGen).ToString());
					}
				}
			}

			Logger.GeneratedThingsReport(parms.traderDef.defName, outThings);
		}
	}
}