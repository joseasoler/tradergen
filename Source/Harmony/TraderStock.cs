using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using TG.Mod;
using TG.StockGen;
using Verse;

namespace TG.Harmony
{
	/// <summary>
	/// Take into account extra stock generators from TraderGen when generating trader stock.
	/// </summary>
	public static class TraderStock
	{
		/// <summary>
		/// Applies Harmony patches required for this functionality.
		/// </summary>
		/// <param name="harmony">Harmony instance.</param>
		public static void Patch(HarmonyLib.Harmony harmony)
		{
			var method = typeof(ThingSetMaker_TraderStock).GetMethod(nameof(ThingSetMaker_TraderStock.Generate),
				BindingFlags.NonPublic | BindingFlags.Instance, null, new[] {typeof(ThingSetMakerParams), typeof(List<Thing>)}, null);
			var prefix = new HarmonyMethod(AccessTools.Method(typeof(TraderStock), nameof(GeneratePrefix)));
			harmony.Patch(method, prefix);
		}

		/// <summary>
		/// Similar to the vanilla implementation in ThingSetMaker_TraderStock.Generate, but it may use StockGenerators from
		/// additional sources.
		/// Logs a generated things report if requested.
		/// </summary>
		/// <param name="parms">Group of parameters for the generation</param>
		/// <param name="outThings">List of generated things</param>
		/// <returns>False, this prefix takes over the generation process.</returns>
		private static bool GeneratePrefix(ThingSetMakerParams parms, List<Thing> outThings)
		{
			Logger.Gen($"Generating stock for {parms.traderDef.defName}.");
			var gens = new List<StockGenerator>();
			gens.AddRange(parms.traderDef.stockGenerators);

			var tile = parms.tile ?? (Find.AnyPlayerHomeMap == null
				? Find.CurrentMap == null ? -1 : Find.CurrentMap.Tile
				: Find.AnyPlayerHomeMap.Tile);

			foreach (var gen in gens)
			{
				if (gen.GetType().IsSubclassOf(typeof(TG.StockGen.StockGen)))
				{
					((TG.StockGen.StockGen) gen).BeforeGen(tile, parms.makingFaction);
				}

				if (Settings.LogGen && Settings.LogStockGen)
				{
					Logger.Gen(Util.ToText(gen).ToString());
				}


				foreach (var thing in gen.GenerateThings(tile, parms.makingFaction))
				{
					if (!thing.def.tradeability.TraderCanSell())
					{
						Log.Error(
							$"{parms.traderDef.defName} used {gen.GetType().Name} to generate {thing.def.defName} which can't be sold by traders. Ignoring...");
					}
					else
					{
						thing.PostGeneratedForTrader(parms.traderDef, tile, parms.makingFaction);

						outThings.Add(thing);
					}
				}
			}

			Logger.GeneratedThingsReport(parms.traderDef.defName, outThings);
			return false;
		}
	}
}