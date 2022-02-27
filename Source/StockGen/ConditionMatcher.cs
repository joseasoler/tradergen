using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace TG.StockGen
{
	/// <summary>
	/// Purchases any item matching a specific criteria.
	/// Sells thingDefCountRange random types of thingDefs matching the criteria, with countRange in stock for each one.
	/// </summary>
	public abstract class ConditionMatcher : StockGenerator
	{
		/// <summary>
		/// Number of thingDefs to choose and generate.
		/// </summary>
		public IntRange thingDefCountRange = IntRange.zero;

		/// <summary>
		/// Returns true for any items which can be purchased.
		/// </summary>
		/// <param name="def">Thing to check.</param>
		/// <returns>If the item can be bought or not.</returns>
		protected abstract bool CanBuy(in ThingDef def);

		/// <summary>
		/// Returns true for any items which can be sold. By default it uses the same condition used for purchases.
		/// </summary>
		/// <param name="def">Thing to check</param>
		/// <param name="forTile">Tile in which the transaction takes place.</param>
		/// <param name="faction">Faction of the trader.</param>
		/// <returns>If the item an be sold or not.</returns>
		protected bool CanSell(in ThingDef def, in int forTile, in Faction faction)
		{
			return CanBuy(def);
		}

		/// <summary>
		/// In addition to the conditions specified in CanBuy, only purchase items which can be sold by the player.
		/// </summary>
		/// <param name="def">Thing to check.</param>
		/// <returns>If the item can be bought or not.</returns>
		private bool CanBuyImpl(in ThingDef def)
		{
			return CanBuy(def) && def.tradeability.PlayerCanSell();
		}

		/// <summary>
		/// In addition to the conditions specified in CanSell, only sell items which can be acquired by the player,
		/// and can be sold by traders.
		/// </summary>
		/// <param name="def">Thing to check</param>
		/// <param name="forTile">Tile in which the transaction takes place.</param>
		/// <param name="faction">Faction of the trader.</param>
		/// <returns>If the item an be sold or not.</returns>
		private bool CanSellImpl(in ThingDef def, in int forTile, in Faction faction)
		{
			return CanSell(def, forTile, faction) && def.tradeability.TraderCanSell() && def.PlayerAcquirable;
		}

		public override IEnumerable<Thing> GenerateThings(int forTile, Faction faction = null)
		{
			var numThingDefsToUse = thingDefCountRange.RandomInRange;
			var thingDefs = DefDatabase<ThingDef>.AllDefs
				.Where(d => CanSellImpl(d, forTile, faction)).ToList();
			var chosenThingDefs = new HashSet<ThingDef>();
			while (numThingDefsToUse > 0 && thingDefs.Count > 0)
			{
				var index = Rand.Range(0, thingDefs.Count);
				var randomThingDef = thingDefs[index];
				if (chosenThingDefs.Contains(randomThingDef)) continue;
				chosenThingDefs.Add(randomThingDef);
				thingDefs.RemoveAt(index);
				--numThingDefsToUse;
			}

			foreach (var chosenThingDef in chosenThingDefs)
			{
				foreach (var thing in StockGeneratorUtility.TryMakeForStock(chosenThingDef, RandomCountOf(chosenThingDef),
					faction))
				{
					yield return thing;
				}
			}
		}

		public override bool HandlesThingDef(ThingDef thingDef)
		{
			return CanBuyImpl(thingDef);
		}
	}
}