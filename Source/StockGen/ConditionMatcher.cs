using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace TG.StockGen
{
	/// <summary>
	/// Sorts weighted ThingDefs in descending order to reduce the average number of iterations for finding an item.
	/// </summary>
	internal class WeightedThingDefComparer : IComparer<Tuple<float, ThingDef>>
	{
		public int Compare(Tuple<float, ThingDef> x, Tuple<float, ThingDef> y)
		{
			if (x == null || y == null)
			{
				throw new ArgumentException("WeightedThingDefComparer: unexpected null parameter.");
			}

			// Elements with larger weights go first.
			var (xWeight, xDef) = x;
			var (yWeight, yDef) = y;
			var result = yWeight.CompareTo(xWeight);

			if (result == 0)
			{
				result = xDef.shortHash.CompareTo(yDef.shortHash);
			}

			return result;
		}
	}

	/// <summary>
	/// Purchases any item matching a specific criteria.
	/// Sells thingDefCountRange random types of thingDefs matching the criteria, with countRange in stock for each one.
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

		protected virtual float Weight(in ThingDef def, in int forTile, in Faction faction) => 1f;

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

		public override bool HandlesThingDef(ThingDef thingDef)
		{
			return CanBuyImpl(thingDef);
		}
	}
}