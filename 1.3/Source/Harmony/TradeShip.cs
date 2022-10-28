using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using TG.TraderKind;
using Verse;

namespace TG.Harmony
{
	/// <summary>
	/// Handles Orbital Trade Ship information.
	/// </summary>
	[HarmonyPatch]
	public static class TradeShipGen
	{
		/// <summary>
		/// Apply the Harmony patches for orbital trader compatibility. Some patches are conditionally applied only if
		/// the Trader Ships mod is not loaded.
		/// </summary>
		/// <param name="harmony">Harmony library instance.</param>
		public static void Patch(HarmonyLib.Harmony harmony)
		{
			var tradeShipConstructor =
				AccessTools.Constructor(typeof(TradeShip), new[] {typeof(TraderKindDef), typeof(Faction)});
			var tradeShipGeneration =
				new HarmonyMethod(AccessTools.Method(typeof(TradeShipGen), nameof(TradeShipGeneration)));
			harmony.Patch(tradeShipConstructor, postfix: tradeShipGeneration);

			var tradeShipFullTitle = AccessTools.PropertyGetter(typeof(TradeShip), nameof(TradeShip.FullTitle));
			var tradeShipNamePatch = new HarmonyMethod(AccessTools.Method(typeof(TradeShipGen), nameof(TradeShipLabel)));
			harmony.Patch(tradeShipFullTitle, postfix: tradeShipNamePatch);
			var tradeShipCallLabel = AccessTools.Method(typeof(TradeShip), nameof(TradeShip.GetCallLabel));
			harmony.Patch(tradeShipCallLabel, postfix: tradeShipNamePatch);

			var tradeShipArrival = AccessTools.Method(typeof(IncidentWorker_OrbitalTraderArrival),
				nameof(IncidentWorker_OrbitalTraderArrival.TryExecuteWorker));
			var tradeShipArrivalLabel =
				new HarmonyMethod(AccessTools.Method(typeof(TradeShipGen), nameof(TradeShipArrivalLabel)));
			harmony.Patch(tradeShipArrival, transpiler: tradeShipArrivalLabel);

			if (HarmonyUtils.TraderShipsEnabled())
			{
				return;
			}

			// See Mod.TraderShips for details.
			var tradeShipExposeData = AccessTools.Method(typeof(TradeShip), nameof(TradeShip.ExposeData));
			var tradeShipLoad =
				new HarmonyMethod(AccessTools.Method(typeof(TradeShipGen), nameof(TradeShipLoad)));
			harmony.Patch(tradeShipExposeData, postfix: tradeShipLoad);

			var tradeShipDepart = AccessTools.Method(typeof(TradeShip), nameof(TradeShip.Depart));
			var tradeShipClear =
				new HarmonyMethod(AccessTools.Method(typeof(TradeShipGen), nameof(TradeShipClear)));
			harmony.Patch(tradeShipDepart, postfix: tradeShipClear);
		}

		/// <summary>
		/// Sets the seed that will be used for random generation of orbital trader information.
		/// Patches the name of the orbital trader.
		/// Temporarily changes the label of the TraderKindDef so
		/// </summary>
		/// <param name="def">Constructor parameter.</param>
		/// <param name="faction">Constructor parameter.</param>
		/// <param name="__instance">TradeShip instance.</param>
		private static void TradeShipGeneration(TraderKindDef def, Faction faction, ref TradeShip __instance)
		{
			Cache.SetSeed(__instance);
			// The incident worker must know specializations to generate the letter. So for trade ships the cache is filled in
			// earlier.
			Cache.TryAdd(__instance.def, __instance.Map?.Tile ?? -1, faction);

			var name = Cache.Name(__instance);
			if (name != null)
			{
				__instance.name = name;
			}
		}

		/// <summary>
		/// Obtains the call label of this ship.
		/// </summary>
		/// <param name="__instance">Trade Ship.</param>
		private static void TradeShipLabel(TradeShip __instance, ref string __result)
		{
			var label = Util.Label(__instance);
			__result = $"{__instance.name} ({label})";
		}

		/// <summary>
		/// Update TraderKind.Cache when a trade ship is loaded.
		/// </summary>
		/// <param name="__instance">TradeShip instance.</param>
		private static void TradeShipLoad(TradeShip __instance)
		{
			// Wait until Map and PassingShipManager are fully loaded before using them.
			if (Scribe.mode != LoadSaveMode.PostLoadInit) return;
			Cache.SetSeed(__instance);
			Cache.TryAdd(__instance.def, __instance.Map?.Tile ?? -1, __instance.Faction);
		}

		/// <summary>
		/// Remove trader information of the settlement from the cache.
		/// </summary>
		/// <param name="__instance">Pawn instance</param>
		private static void TradeShipClear(TradeShip __instance)
		{
			Cache.Remove(__instance);
		}

		private static void SendOrbitalTraderArrivalLetter(IncidentWorker_OrbitalTraderArrival worker, IncidentParms parms,
			TradeShip trader)
		{
			var name = trader.name;
			var label = trader.def.label;
			var factionStr = trader.Faction == null
				? "TraderArrivalNoFaction".Translate()
				: "TraderArrivalFromFaction".Translate(trader.Faction.Named("FACTION"));

			var specializations = Cache.Specializations(trader);

			TaggedString letterText;
			if (specializations.NullOrEmpty())
			{
				letterText = "TraderArrival".Translate(name, label, factionStr);
			}
			else
			{
				var specializationsStr = "";
				for (var index = 0; index < specializations.Count - 1; ++index)
				{
					if (index > 0)
					{
						specializationsStr += ", ";
					}

					specializationsStr += specializations[index].label;
				}

				if (specializations.Count > 1)
				{
					specializationsStr += ' ' + "AndLower".Translate() + ' ';
				}

				specializationsStr += specializations[specializations.Count - 1].label;

				letterText = "TraderArrivalSpecializations".Translate(name, label, specializationsStr, factionStr);
			}

			worker.SendStandardLetter(Util.Label(trader).CapitalizeFirst(), letterText, LetterDefOf.PositiveEvent, parms,
				LookTargets.Invalid);
		}

		private static IEnumerable<CodeInstruction> TradeShipArrivalLabel(IEnumerable<CodeInstruction> instructions)
		{
			var getFaction = AccessTools.Method(typeof(IncidentWorker_OrbitalTraderArrival),
				nameof(IncidentWorker_OrbitalTraderArrival.GetFaction));
			var startLookingForInjectionPoint = false;
			var injectionStart = false;
			var injectionEnd = false;
			foreach (var code in instructions)
			{
				// After getFaction is called, look for the injection point.
				startLookingForInjectionPoint = startLookingForInjectionPoint || code.Calls(getFaction);
				// Detect the first line that should be replaced.
				if (startLookingForInjectionPoint && code.opcode == OpCodes.Ldloc_1 && !injectionStart)
				{
					injectionStart = true;
					// Inject the new code. IncidentWorker_OrbitalTraderArrival should have been loaded right before.
					yield return new CodeInstruction(OpCodes.Ldarg_1); // IncidentParms.
					yield return new CodeInstruction(OpCodes.Ldloc_1); // TradeShip.
					yield return new CodeInstruction(OpCodes.Call,
						AccessTools.Method(typeof(TradeShipGen), nameof(SendOrbitalTraderArrivalLetter)));
				}

				// Detects the first line that should not be replaced.
				injectionEnd = injectionEnd || injectionStart && code.opcode == OpCodes.Ldloc_0;

				if (!injectionStart || injectionEnd)
				{
					yield return code;
				}
			}
		}

		/// <summary>
		/// The vanilla TradeShip.GenerateThings implementation does not pass the faction for some reason.
		/// </summary>
		/// <param name="__instance">Trade ship.</param>
		/// <returns>Always false. TraderGen takes over the execution of this function.</returns>
		[HarmonyPrefix]
		[HarmonyPatch(typeof(TradeShip), nameof(TradeShip.GenerateThings))]
		private static bool TradeShipGeneratePrefix(TradeShip __instance)
		{
			__instance.things.TryAddRangeOrTransfer(ThingSetMakerDefOf.TraderStock.root.Generate(new ThingSetMakerParams
			{
				traderDef = __instance.def,
				tile = __instance.Map.Tile,
				makingFaction = __instance.Faction
			}));
			return false;
		}
	}
}