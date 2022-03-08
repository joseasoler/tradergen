using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace TG.StockGen
{
	/// <summary>
	/// Purchases any item matching a specific criteria.
	/// Sells thingDefCountRange random types of thingDefs matching the criteria, with countRange/totalPriceRange in stock
	/// for each one.
	/// ThingDef are randomly chosen using a customizable weight.
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
		protected virtual bool CanSell(in ThingDef def, in int forTile, in Faction faction)
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
			return CanBuy(def) && def.tradeability.PlayerCanSell() && def.techLevel <= maxTechLevelBuy;
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
			return CanSell(def, forTile, faction) && def.tradeability.TraderCanSell() && def.PlayerAcquirable &&
			       def.techLevel <= maxTechLevelGenerate;
		}

		/// <summary>
		/// Defines the weight of a def when randomly choosing which defs to generate.
		/// </summary>
		/// <param name="def">Thing being checked</param>
		/// <param name="forTile">Tile in which the transaction takes place.</param>
		/// <param name="faction">Faction of the trader.</param>
		/// <returns></returns>
		protected virtual float Weight(in ThingDef def, in int forTile, in Faction faction) => 1f;

		/// <summary>
		/// Sells thingDefCountRange random thingDefs matching the criteria, with countRange in stock for each one.
		/// </summary>
		/// <param name="forTile">Tile in which the transaction takes place.</param>
		/// <param name="faction">Faction of the trader.</param>
		/// <returns></returns>
		public override IEnumerable<Thing> GenerateThings(int forTile, Faction faction = null)
		{
			var thingDefs = DefDatabase<ThingDef>.AllDefs.Where(def => CanSellImpl(def, forTile, faction)).ToList();
			var chosenThingDefs = Algorithm.ChooseNWeightedRandomly(thingDefs, def => Weight(def, forTile, faction),
				thingDefCountRange.RandomInRange);

			foreach (var thing in chosenThingDefs.SelectMany(def =>
				         StockGeneratorUtility.TryMakeForStock(def, RandomCountOf(def), faction)))
			{
				yield return thing;
			}
		}

		/// <summary>
		/// Checks if the trader can purchase a given item.
		/// </summary>
		/// <param name="thingDef">ThingDef being checked.</param>
		/// <returns>True if the trader will buy the item.</returns>
		public override bool HandlesThingDef(ThingDef thingDef)
		{
			return CanBuyImpl(thingDef);
		}
	}
}