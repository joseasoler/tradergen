using System.Collections.Generic;
using RimWorld;
using Verse;

namespace TG.StockGen
{
	public class Artillery : ConditionMatcher
	{
		/// <summary>
		/// Will never generate any ThingDefs included in this list.
		/// </summary>
		public List<ThingDef> excludeThingDefs;

		/// <summary>
		/// Whenever possible, create all artillery using steel.
		/// </summary>
		/// <param name="def">ThingDef being generated.</param>
		/// <param name="faction">Faction of the trader.</param>
		/// <returns>Things generated for this ThingDef.</returns>
		protected override IEnumerable<Thing> TryMakeForStock(ThingDef def, Faction faction)
		{
			if (!def.tradeability.TraderCanSell())
			{
				Logger.ErrorOnce($"TG.StockGen.Artillery tried to make non-trader-sellable thing {def} for trader stock.");
				yield break;
			}

			if (def.MadeFromStuff && ThingDefOf.Steel.stuffProps.CanMake(def))
			{
				var stackCount = RandomCountOf(def);
				for (var index = 0; index < stackCount; ++index)
				{
					var thing = ThingMaker.MakeThing(def, ThingDefOf.Steel);
					thing.stackCount = 1;
					yield return thing;
				}
				yield break;
			}

			foreach (var thing in base.TryMakeForStock(def, faction))
			{
				yield return thing;
			}
		}

		protected override bool CanBuy(in ThingDef def)
		{
			return def.IsWithinCategory(DefOf.ThingCategory.BuildingsSecurity) && def.building?.buildingTags != null &&
			       def.building.buildingTags.Contains("Artillery") && def.Minifiable &&
			       (excludeThingDefs == null || !excludeThingDefs.Contains(def));
		}
	}
}