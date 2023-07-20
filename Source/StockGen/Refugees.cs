using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace TraderGen.StockGen
{
	/// <summary>
	/// Generates a group of desperate refugees, with baseliners being relatively infrequent. Frequently, these refugees
	/// know each other.
	/// </summary>
	public class Refugees : StockGen
	{
		private static bool ValidXenotypeDef(XenotypeDef def)
		{
			return def.factionlessGenerationWeight > 0.0F;
		}

		private static float XenotypeDefWeight(XenotypeDef def)
		{
			return def == XenotypeDefOf.Baseliner ? 4.0F : def.factionlessGenerationWeight;
		}

		private static XenotypeDef RandomXenotype()
		{
			// Choose a random humanlike faction definition.
			DefDatabase<XenotypeDef>.AllDefsListForReading.Where(ValidXenotypeDef)
				.TryRandomElementByWeight(XenotypeDefWeight, out var xenotypeDef);
			return xenotypeDef;
		}

		private static Pawn GenerateColonistPawn(PawnGenerationRequest request)
		{
			var pawn = PawnGenerator.GeneratePawn(request);
			if (pawn.guest != null && pawn.guest.joinStatus == JoinStatus.JoinAsSlave)
			{
				pawn.guest.joinStatus = JoinStatus.JoinAsColonist;
			}

			return pawn;
		}

		public override IEnumerable<Thing> GenerateThings(int forTile, Faction faction = null)
		{
			var developmentalStages = Find.Storyteller.difficulty.ChildrenAllowed
				? DevelopmentalStage.Child | DevelopmentalStage.Adult
				: DevelopmentalStage.Adult;

			var refugeeFaction =
				!Find.FactionManager.TryGetRandomNonColonyHumanlikeFaction(out Faction randomFaction, false, true)
					? Faction.OfAncients
					: randomFaction;


			var request = new PawnGenerationRequest(PawnKindDefOf.Colonist, refugeeFaction, tile: forTile,
				forceGenerateNewPawn: true, colonistRelationChanceFactor: 0.0F, allowPregnant: true,
				forceAddFreeWarmLayerIfNeeded: true, forcedXenotype: RandomXenotype(),
				developmentalStages: developmentalStages);

			var refugeeCount = countRange.RandomInRange;
			for (var current = 0; current < refugeeCount; ++current)
			{
				yield return GenerateColonistPawn(request);
			}
		}

		public override bool HandlesThingDef(ThingDef thingDef)
		{
			return thingDef.category == ThingCategory.Pawn && thingDef.race.Humanlike &&
				thingDef.tradeability != Tradeability.None;
		}
	}
}