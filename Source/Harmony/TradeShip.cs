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
			__instance.def = Generator.Def(def, __instance.RandomPriceFactorSeed, __instance.Map?.Tile ?? -1,
				__instance.faction);

			var name = Generator.Name(__instance.def, __instance.faction);
			if (name != null)
			{
				__instance.name = name;
			}
		}

		/// <summary>
		/// Regenerate the TraderKindDef after loading a trade ship.
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
				__instance.def = Generator.Def(__instance.def, __instance.RandomPriceFactorSeed, __instance.Map?.Tile ?? -1,
					__instance.faction);
			}
		}
	}
}