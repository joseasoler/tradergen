using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using TG.Mod;
using TraderGen.TraderKind;
using Verse;

namespace TraderGen.Harmony.StockGeneration
{

	[HarmonyPatch(typeof(ThingSetMaker_TraderStock), nameof(ThingSetMaker_TraderStock.Generate))]
	internal static class ThingSetMaker_TraderStock_Generate_Patch
	{
		/// <summary>
		/// Prepares the extra TraderGen information. This must be done before vanilla content is generated as TraderGen
		/// may need to forbid some of the stock they create.
		/// The seed must have been set beforehand by patches in TradeShip, Pawn or Settlement.
		/// </summary>
		private static void Prefix(ThingSetMakerParams parms, ref List<Thing> outThings)
		{
			Cache.TryAdd(parms.traderDef, parms.tile ?? -1, parms.makingFaction);
		}
		
		/// <summary>
		/// Generates extra TraderGen stock after generating the vanilla stock.
		/// Logs a generated things report if requested.
		/// </summary>
		private static void Postfix(ThingSetMakerParams parms, ref List<Thing> outThings)
		{
			TraderKindDef trader = parms.traderDef;
			int tile = parms.tile ?? -1;
			Faction faction = parms.makingFaction;

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