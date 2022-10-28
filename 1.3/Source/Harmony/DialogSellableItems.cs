using System.Linq;
using HarmonyLib;
using RimWorld;
using TG.TraderKind;
using Verse;

namespace TG.Harmony
{
	/// <summary>
	/// Patches the sellable items dialog to take into account TraderGen additions.
	/// </summary>
	[HarmonyPatch]
	public static class DialogSellableItems
	{
		/// <summary>
		/// TraderGen takes over this calculation.
		/// </summary>
		[HarmonyPrefix]
		[HarmonyPatch(typeof(Dialog_SellableItems), nameof(Dialog_SellableItems.CalculateSellableItems))]
		private static bool DisableCalculateSellableItems()
		{
			return false;
		}

		/// <summary>
		/// Use the TraderGen WillTrade cache to calculate sellable items.
		/// </summary>
		/// <param name="__instance">Sellable items dialog.</param>
		[HarmonyPostfix]
		[HarmonyPatch(typeof(Dialog_SellableItems), MethodType.Constructor, typeof(ITrader))]
		private static void CalculateSellableItems(Dialog_SellableItems __instance)
		{
			__instance.cachedSellableItemsByCategory.Clear();
			__instance.cachedSellablePawns = null;

			__instance.sellableItems = DefDatabase<ThingDef>.AllDefsListForReading.Where(
				thing => thing.PlayerAcquirable && !thing.IsCorpse &&
				         !typeof(MinifiedThing).IsAssignableFrom(thing.thingClass) &&
				         Cache.WillTrade(__instance.trader, __instance.trader.TraderKind, thing) &&
				         TradeUtility.EverPlayerSellable(thing)).ToList();

			__instance.sellableItems.SortBy(x => x.label);
		}
	}
}