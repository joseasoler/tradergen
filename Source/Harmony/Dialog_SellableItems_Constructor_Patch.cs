using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using TraderGen.TraderKind;
using Verse;

namespace TraderGen.Harmony
{
	/// <summary>
	/// Use the TraderGen WillTrade cache to calculate sellable items.
	/// </summary>
	[HarmonyPatch(typeof(Dialog_SellableItems), MethodType.Constructor, typeof(ITrader))]
	internal static class Dialog_SellableItems_Constructor_Patch
	{
		private static bool IsSellable(ITrader trader, ThingDef thingDef)
		{
			return thingDef.PlayerAcquirable && !thingDef.IsCorpse &&
				!typeof(MinifiedThing).IsAssignableFrom(thingDef.thingClass) &&
				Cache.WillTrade(trader, trader.TraderKind, thingDef) &&
				TradeUtility.EverPlayerSellable(thingDef);
		}

		private static void Postfix(Dialog_SellableItems __instance)
		{
			__instance.cachedSellableItemsByCategory.Clear();
			__instance.cachedSellablePawns = null;

			List<ThingDef> thingDefs = DefDatabase<ThingDef>.AllDefsListForReading;
			__instance.sellableItems = new List<ThingDef>();

			for (int thingDefIndex = 0; thingDefIndex < thingDefs.Count; ++thingDefIndex)
			{
				ThingDef thingDef = thingDefs[thingDefIndex];
				if (IsSellable(__instance.trader, thingDef))
				{
					__instance.sellableItems.Add(thingDef);
				}
			}

			__instance.sellableItems.SortBy(x => x.label);
		}
	}
}