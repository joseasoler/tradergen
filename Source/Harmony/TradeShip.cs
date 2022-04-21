using System.Linq;
using HarmonyLib;
using RimWorld;
using TG.TraderKind;
using Verse;

namespace TG.Harmony
{
	/// <summary>
	/// Patches required for orbital trader compatibility.
	/// </summary>
	[HarmonyPatch]
	public static class TradeShipGen
	{
		/// <summary>
		/// Replaces the chosen TraderKindDef with a shallow copy with all required StockGenerators properly initialized. 
		/// </summary>
		/// <param name="def">Constructor parameter.</param>
		/// <param name="faction">Constructor parameter.</param>
		/// <param name="__instance">TradeShip instance.</param>
		[HarmonyPostfix]
		[HarmonyPatch(typeof(TradeShip), MethodType.Constructor, typeof(TraderKindDef), typeof(Faction))]
		private static void ConstructTraderKindDef(TraderKindDef def, Faction faction, ref TradeShip __instance)
		{
			__instance.def = Generator.Generate(def, __instance.RandomPriceFactorSeed, __instance.Map?.Tile ?? -1,
				__instance.faction);
		}

		/// <summary>
		/// Regenerate the TraderKindDef after loading. 
		/// Since the generated TraderKindDef has the same defName as the real one, TradeShip.ExposeData will "save"
		/// the real TraderKindDef. When it is loaded, it will also get the real TraderKindDef. After the loading process is
		/// finished, it is replaced with the generated TraderKindDef.
		/// </summary>
		/// <param name="__instance">TradeShip instance.</param>
		[HarmonyPostfix]
		[HarmonyPatch(typeof(TradeShip), nameof(TradeShip.ExposeData))]
		private static void LoadTraderKindDef(ref TradeShip __instance)
		{
			// Wait until Map and PassingShipManager are fully loaded before using them.
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				__instance.def = Generator.Generate(__instance.def, __instance.RandomPriceFactorSeed,
					__instance.Map?.Tile ?? -1, __instance.faction);
			}
		}


		/*

		/// <summary>
		/// Regenerates all TraderKindDefs associated to TradeShips after loading. 
		/// Since the generated TraderKindDefs must have the same defName as a real one, TradeShip.ExposeData will work
		/// correctly but it load a real TraderKindDefs instead of a generated one.
		/// Since Generator.Generate must know the tile of the transaction and that is initialized by PassingShipManager,
		/// the TraderKindDefs are regenerated here instead.
		/// </summary>
		/// <param name="__instance">TradeShip instance.</param>
		[HarmonyPostfix]
		[HarmonyPatch(typeof(PassingShipManager), nameof(PassingShipManager.ExposeData))]
		private static void LoadTraderKindDefs(ref PassingShipManager __instance)
		{
			// Wait until the map is fully loaded before using it.
			if (Scribe.mode != LoadSaveMode.PostLoadInit)
			{
				return;
			}

			foreach (var tradeShip in __instance.passingShips.Cast<TradeShip>().Where(tradeShip => tradeShip != null))
			{
				tradeShip.def = Generator.Generate(tradeShip.def, tradeShip.RandomPriceFactorSeed, tradeShip.Map?.Tile ?? -1,
					tradeShip.faction);
			}
		}
		 */
	}
}