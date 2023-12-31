﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace TraderGen.StockGen
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
			base.ToText(ref b);

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
				yield return "TraderGen.StockGen.ConditionMatcher: minTechLevelGenerate is greater than maxTechLevelGenerate.";
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

		protected virtual bool ValidTechLevel(in ThingDef def)
		{
			return def.techLevel >= minTechLevelGenerate && def.techLevel <= maxTechLevelGenerate;
		}

		/// <summary>
		/// In addition to the conditions specified in CanSell, only sell items which can be acquired by the player,
		/// and can be sold by traders.
		/// </summary>
		/// <param name="def">Thing to check</param>
		/// <param name="forTile">Tile in which the transaction takes place.</param>
		/// <param name="faction">Faction of the trader.</param>
		/// <returns>If the item can be sold or not.</returns>
		private bool CanSellImpl(in ThingDef def, in int forTile, in Faction faction)
		{
			return CanSell(def, forTile, faction) && def.tradeability == Tradeability.All && def.PlayerAcquirable &&
				ValidTechLevel(def);
		}

		/// <summary>
		/// Defines the weight of a def when randomly choosing which defs to generate.
		/// </summary>
		/// <param name="def">Thing being checked</param>
		/// <param name="forTile">Tile in which the transaction takes place.</param>
		/// <param name="faction">Faction of the trader.</param>
		/// <returns>Random weight of the def.</returns>
		protected virtual float Weight(in ThingDef def, in int forTile, in Faction faction) => 1.0F;

		/// <summary>
		/// Generates stock for a specific ThingDef.
		/// </summary>
		/// <param name="thingDef">Thing being generated.</param>
		/// <param name="faction">Faction of the trader.</param>
		/// <returns>Generated things.</returns>
		protected virtual IEnumerable<Thing> TryMakeForStock(ThingDef thingDef, Faction faction)
		{
			// The default implementation mimics StockGeneratorUtility.TryMakeForStock with some optimizations.
			if (!thingDef.tradeability.TraderCanSell())
			{
				Log.Error($"Tried to make non-trader-sellable thing {thingDef} for trader stock.");
				yield break;
			}

			int count = RandomCountOf(thingDef);
			if (count <= 0)
			{
				yield break;
			}

			int stackCount;
			int times;
			if (thingDef.MadeFromStuff || thingDef.tradeNeverStack || thingDef.tradeNeverGenerateStacked)
			{
				stackCount = 1;
				times = count;
			}
			else
			{
				stackCount = count;
				times = 1;
			}

			// The list of available stuff ThingDefs is only generated once.
			List<ThingDef> stuffs = null;
			if (thingDef.MadeFromStuff)
			{
				stuffs = new List<ThingDef>();
				foreach (ThingDef stuffDef in GenStuff.AllowedStuffsFor(thingDef))
				{
					if (!PawnWeaponGenerator.IsDerpWeapon(thingDef, stuffDef) && stuffDef.stuffProps.commonality > 0)
					{
						stuffs.Add(stuffDef);
					}
				}

				if (stuffs.Count == 0)
				{
					stuffs = new List<ThingDef> { GenStuff.DefaultStuffFor(thingDef) };
				}
			}

			for (int generationIndex = 0; generationIndex < times; ++generationIndex)
			{
				ThingDef stuff = null;
				if (stuffs != null)
				{
					stuff = stuffs.Count > 1 ? stuffs.RandomElementByWeight(x => x.stuffProps.commonality) : stuffs[0];
				}

				Thing thing = ThingMaker.MakeThing(thingDef, stuff);
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
			List<ThingDef> thingDefs = new List<ThingDef>();
			foreach (ThingDef thingDef in DefDatabase<ThingDef>.AllDefsListForReading)
			{
				if (CanSellImpl(thingDef, forTile, faction))
				{
					thingDefs.Add(thingDef);
				}
			}

			List<ThingDef> chosenThingDefs = Algorithm.ChooseNWeightedRandomly(thingDefs,
				def => Weight(def, forTile, faction),
				thingDefCountRange.RandomInRange);

			foreach (ThingDef thingDef in chosenThingDefs)
			{
				foreach (Thing thing in TryMakeForStock(thingDef, faction))
				{
					yield return thing;
				}
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