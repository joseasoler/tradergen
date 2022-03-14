using System.Collections.Generic;
using System.Text;
using RimWorld;
using Verse;

namespace TG.StockGen
{
	/// <summary>
	/// Generates a specific ThingDef with stuffProps, and items which can be made from the same stuff.
	/// </summary>
	public abstract class FromStuff : ConditionMatcher
	{
		public IntRange stuffCountRange;

		protected ThingDef _stuffDef;

		public ThingDef StuffDef => _stuffDef;

		public override void ConditionToText(ref StringBuilder b)
		{
			base.ConditionToText(ref b);
			b.Append($"stuffCountRange: {stuffCountRange}\n");
		}

		/// <summary>
		/// Used by derived classes to choose the ThingDef to use as material.
		/// </summary>
		protected abstract void SetStuffDef();

		/// <summary>
		/// Takes advantage of the ResolveReferences call in ProcGen to choose the stuff to use as material.
		/// </summary>
		/// <param name="newTrader">TraderKindDef being associated with this StockGenerator.</param>
		public override void ResolveReferences(TraderKindDef newTrader)
		{
			base.ResolveReferences(newTrader);
			SetStuffDef();
		}

		public override IEnumerable<string> ConfigErrors(TraderKindDef parentDef)
		{
			foreach (var err in base.ConfigErrors(parentDef))
			{
				yield return err;

			}

			if (_stuffDef == null)
			{
				yield return "TG.StockGen.FromStuff has a null ThingDef.";
			}
			else if (!_stuffDef.IsStuff)
			{
				yield return $"TG.StockGen.FromStuff is using a ThingDef which is not Stuff: {_stuffDef.defName}.";
			}
		}

		/// <summary>
		/// Generates stock using a ThingDef.
		/// </summary>
		/// <param name="def">Thing being generated.</param>
		/// <param name="faction">Faction of the trader.</param>
		/// <returns></returns>
		protected override IEnumerable<Thing> TryMakeForStock(ThingDef def, Faction faction)
		{
			if (!def.tradeability.TraderCanSell() || !def.MadeFromStuff)
			{
				Logger.ErrorOnce($"TG.StockGen.FromStuff cannot generate {def} for trader stock.");
				yield break;
			}

			var stackCount = RandomCountOf(def);
			for (var index = 0; index < stackCount; ++index)
			{
				var thing = ThingMaker.MakeThing(def, _stuffDef);
				thing.stackCount = 1;
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
			if (_stuffDef.tradeability.TraderCanSell())
			{
				foreach (var thing in StockGeneratorUtility.TryMakeForStock(_stuffDef, stuffCountRange.RandomInRange, faction))
				{
					yield return thing;
				}
			}

			foreach (var thing in base.GenerateThings(forTile, faction))
			{
				yield return thing;
			}
		}

		/// <summary>
		/// Purchases the chosen stuff and anything which could potentially be made from the same stuff.
		/// Weapons are handled in other StockGens.
		/// </summary>
		/// <param name="def">ThingDef being checked.</param>
		/// <returns>True if the trader will buy the item.</returns>
		protected override bool CanBuy(in ThingDef def)
		{
			return def == _stuffDef ||
			       // Only accept material which could be made from this stuff.
			       _stuffDef.stuffProps.CanMake(def) &&
			       // Only sell non-armor apparel...
			       (def.IsApparel && !Util.IsArmor(def)
			        // ... minifiable buildings which are not security buildings...
			        || def.IsWithinCategory(ThingCategoryDefOf.Buildings) && def.Minifiable && (def.thingCategories == null ||
				        !def.thingCategories.Contains(DefOf.ThingCategory.BuildingsSecurity)));
		}

		/// <summary>
		/// Defines the weight of a def when randomly choosing which defs to generate.
		/// </summary>
		/// <param name="def">Thing being checked</param>
		/// <param name="forTile">Tile in which the transaction takes place.</param>
		/// <param name="faction">Faction of the trader.</param>
		/// <returns>Random weight of the def.</returns>
		protected override float Weight(in ThingDef def, in int forTile, in Faction faction)
		{
			// Prevent the base material from being chosen again. Its generation is handled separately.
			if (def == _stuffDef)
			{
				return 0.00001f;
			}

			return def.IsApparel
				? StockGenerator_Clothes.SelectionWeightMarketValueCurve.Evaluate(def.BaseMarketValue)
				: StockGenerator_Art.SelectionWeightMarketValueCurve.Evaluate(def.BaseMarketValue);
		}
	}
}