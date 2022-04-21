using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace TG.Harmony
{
	/// <summary>
	/// Generated things report.
	/// </summary>
	[HarmonyPatch]
	public static class TraderStock
	{
		/// <summary>
		/// Logs a generated things report if requested.
		/// </summary>
		/// <param name="parms">Group of parameters to be used for stock generation.</param>
		/// <param name="outThings">List of generated things.</param>
		[HarmonyPostfix]
		[HarmonyPatch(typeof(ThingSetMaker_TraderStock), nameof(ThingSetMaker_TraderStock.Generate))]
		private static void GeneratePostfix(ThingSetMakerParams parms, List<Thing> outThings)
		{
			Logger.GeneratedThingsReport(parms.traderDef.defName, outThings);
		}
	}
}