using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using TG.TraderKind;
using Verse;

namespace TG.Harmony
{
	[HarmonyPatch]
	public class Caravan
	{
		/// <summary>
		/// Inject a modified TraderKindDef into the caravan generation process.
		/// This implementation takes advantage of the fact that GeneratePawns will use parms.traderKind if it is available.
		/// </summary>
		[HarmonyPrefix]
		[HarmonyPatch(typeof(PawnGroupKindWorker_Trader), nameof(PawnGroupKindWorker_Trader.GeneratePawns))]
		private static bool InjectModifiedTrader(ref PawnGroupMakerParms parms, PawnGroupMaker groupMaker,
			List<Pawn> outPawns, bool errorOnZeroResults)
		{
			if (parms.faction == null || parms.faction.def.caravanTraderKinds.Count == 0)
			{
				return true;
			}

			if (parms.traderKind == null)
			{
				parms.traderKind =
					parms.faction.def.caravanTraderKinds.RandomElementByWeight(traderDef => traderDef.CalculatedCommonality);
			}

			// Avoid modifying favor traders.
			if (parms.traderKind.tradeCurrency != TradeCurrency.Favor)
			{
				// Deterministic seed is not implemented for pawn generation.
				parms.traderKind = Generator.Def(parms.traderKind, Rand.Int, parms.tile, parms.faction);
			}

			return true;
		}
	}
}