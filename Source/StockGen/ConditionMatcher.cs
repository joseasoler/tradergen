using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
	public abstract class ConditionMatcher : StockGen
	{
		/// <summary>
		/// Number of thingDefs to choose and generate.
		/// </summary>
		public IntRange thingDefCountRange = IntRange.zero;

		/// <summary>
		/// Will only have in stock items with this tech level or higher. Helpful for removing fluff in orbital traders.
		/// </summary>
		public TechLevel minTechLevelGenerate = TechLevel.Undefined;

		public override void ToText(ref StringBuilder b)
		{
			if (thingDefCountRange != IntRange.zero)
			{
				b.Append($"thingDefCountRange: {thingDefCountRange}\n");
			}

			if (minTechLevelGenerate != TechLevel.Undefined)
			{
				b.Append($"minTechLevelGenerate: {Enum.GetName(typeof(TechLevel), minTechLevelGenerate)}\n");
			}

			ConditionToText(ref b);
		}

		public override IEnumerable<string> ConfigErrors(TraderKindDef parentDef)
		{
			foreach (var err in base.ConfigErrors(parentDef))
			{
				yield return err;
			}

			if (minTechLevelGenerate > maxTechLevelGenerate)
			{
				yield return "TG.StockGen.ConditionMatcher: minTechLevelGenerate is greater than maxTechLevelGenerate.";
			}
		}

		public virtual void ConditionToText(ref StringBuilder b)
		{
		}

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
		/// <returns>If the item can be sold or not.</returns>
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
			return CanSell(def, forTile, faction) && def.tradeability == Tradeability.All && def.PlayerAcquirable &&
			       def.techLevel <= maxTechLevelGenerate && def.techLevel >= minTechLevelGenerate;
		}

		/// <summary>
		/// Defines the weight of a def when randomly choosing which defs to generate.
		/// </summary>
		/// <param name="def">Thing being checked</param>
		/// <param name="forTile">Tile in which the transaction takes place.</param>
		/// <param name="faction">Faction of the trader.</param>
		/// <returns>Random weight of the def.</returns>
		protected virtual float Weight(in ThingDef def, in int forTile, in Faction faction) => 1f;

		/// <summary>
		/// Generates stock for a specific ThingDef.
		/// </summary>
		/// <param name="def">Thing being generated.</param>
		/// <param name="faction">Faction of the trader.</param>
		/// <returns>Generated things.</returns>
		protected virtual IEnumerable<Thing> TryMakeForStock(ThingDef def, Faction faction)
		{
			// The default implementation mimics StockGeneratorUtility.TryMakeForStock with some optimizations.
			if (!def.tradeability.TraderCanSell())
			{
				Log.Error($"Tried to make non-trader-sellable thing {def} for trader stock.");
				yield break;
			}

			var count = RandomCountOf(def);
			if (count <= 0)
			{
				yield break;
			}

			var stackCount = def.MadeFromStuff ? 1 : count;
			var times = def.MadeFromStuff ? count : 1;

			// The list of available stuff ThingDefs is only generated once.
			List<ThingDef> stuffs = null;
			if (def.MadeFromStuff)
			{
				stuffs = GenStuff.AllowedStuffsFor(def).Where(x => !PawnWeaponGenerator.IsDerpWeapon(def, x)).ToList();
				if (stuffs.Count == 0 || stuffs.All(x => x.stuffProps.commonality <= 0))
				{
					stuffs = new List<ThingDef> {GenStuff.DefaultStuffFor(def)};
				}
			}

			for (var idx = 0; idx < times; ++idx)
			{
				ThingDef stuff = null;
				if (stuffs != null)
				{
					stuff = stuffs.Count > 1 ? stuffs.RandomElementByWeight(x => x.stuffProps.commonality) : stuffs[0];
				}

				var thing = ThingMaker.MakeThing(def, stuff);
				thing.stackCount = stackCount;
				yield return thing;
			}
		}

		/// <summary>
		/// Sells thingDefCountRange random thingDefs matching the criteria, with countRange in stock for each one.
		/// </summary>
		/// <param name="forTile">Tile in which the transaction takes place.</param>
		/// <param name="faction">Faction of the trader.</param>
		/// <returns>Stock generated by this object.</returns>
		public override IEnumerable<Thing> GenerateThings(int forTile, Faction faction = null)
		{
			var thingDefs = DefDatabase<ThingDef>.AllDefs.Where(def => CanSellImpl(def, forTile, faction)).ToList();
			var chosenThingDefs = Algorithm.ChooseNWeightedRandomly(thingDefs, def => Weight(def, forTile, faction),
				thingDefCountRange.RandomInRange);

			foreach (var thing in chosenThingDefs.SelectMany(def => TryMakeForStock(def, faction)))
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