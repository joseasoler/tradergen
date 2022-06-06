using HarmonyLib;
using RimWorld;
using TG.TraderKind;
using Verse;

namespace TG.Harmony.Mod
{
	/// <summary>
	/// Compatibility with the Trader Ships mod and its retextures.
	/// </summary>
	public static class TraderShips
	{
		/// <summary>
		/// Apply the Harmony patch for Trader Ships compatibility only if the mod is loaded.
		/// </summary>
		/// <param name="harmony">Harmony library instance.</param>
		public static void Patch(HarmonyLib.Harmony harmony)
		{
			if (!HarmonyUtils.TraderShipsEnabled())
			{
				return;
			}

			var exposeData = AccessTools.Method("TraderShips.LandedShip:ExposeData");
			var traderShipsLoad = new HarmonyMethod(AccessTools.Method(typeof(TraderShips), nameof(TraderShipsLoad)));
			harmony.Patch(exposeData, postfix: traderShipsLoad);

			var sendAway = AccessTools.Method("TraderShips.CompShip:SendAway");
			var traderShipsCleanPostfix =
				new HarmonyMethod(AccessTools.Method(typeof(TraderShips), nameof(TraderShipsClean)));
			harmony.Patch(sendAway, traderShipsCleanPostfix);

			var crash = AccessTools.Method("TraderShips.CompShip:Crash");
			harmony.Patch(crash, postfix: traderShipsCleanPostfix);
		}

		/// <summary>
		/// LandedShips have their own map attribute.
		/// </summary>
		/// <param name="___def">TraderKindDef to generate.</param>
		/// <param name="___randomPriceFactorSeed">Random seed.</param>
		/// <param name="___map">Map in which the ship has landed.</param>
		/// <param name="___faction">Faction of the trader.</param>
		private static void TraderShipsLoad(ref TraderKindDef ___def, int ___randomPriceFactorSeed, ref Map ___map,
			ref Faction ___faction)
		{
			if (Scribe.mode != LoadSaveMode.PostLoadInit) return;
			Cache.SetSeed(___randomPriceFactorSeed);
			Cache.TryAdd(___def, ___map?.Tile ?? -1, ___faction);
		}


		/// <summary>
		/// Remove trader information of the trader from the cache.
		/// </summary>
		/// <param name="___tradeShip">TradeShip attribute</param>
		private static void TraderShipsClean(TradeShip ___tradeShip)
		{
			Cache.Remove(___tradeShip);
		}
	}
}