using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using TG.Mod;
using TG.TraderKind;
using Verse;

namespace TG.Harmony
{
	[HarmonyPatch]
	public static class StockGeneratorCount
	{
		private static Dictionary<ushort, TraderKindCategory>
			_categoryByHash = new Dictionary<ushort, TraderKindCategory>();

		[HarmonyPostfix]
		[HarmonyPatch(typeof(StockGenerator), nameof(StockGenerator.RandomCountOf))]
		private static void ModifyGeneratedAmounts(ref StockGenerator __instance, ref int __result, ThingDef def)
		{
			if (def == ThingDefOf.Silver)
			{
				if (!_categoryByHash.ContainsKey(def.shortHash))
				{
					_categoryByHash[def.shortHash] = Util.GetCategory(__instance.trader);
				}

				var category = _categoryByHash[def.shortHash];
				__result = (int) (Settings.GetSilverScaling(category) * __result / 100.0f);
			}
		}
	}
}