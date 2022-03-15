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
		public IntRange stuffCountRange = IntRange.zero;

		protected ThingDef _stuffDef;

		public ThingDef StuffDef => _stuffDef;

		protected int _fromTile = -1;

		protected Faction _faction = null;

		public override void ConditionToText(ref StringBuilder b)
		{
			base.ConditionToText(ref b);
			if (stuffCountRange != IntRange.zero)
			{
				b.Append($"stuffCountRange: {stuffCountRange}\n");
			}
		}

		/// <summary>
		/// Used by derived classes to choose the ThingDef to use as material.
		/// </summary>
		protected abstract void SetStuffDef();

		/// <summary>
		/// Takes advantage of the SetTraderInfo call in ProcGen to choose the stuff to use as material.
		/// </summary>
		/// <param name="fromTile">Map tile considered as the origin of the trader.</param>
		/// <param name="faction">Faction of the trader.</param>
		public override void SetTraderInfo(in int fromTile, in Faction faction)
		{
			base.SetTraderInfo(fromTile, faction);
			_fromTile = fromTile;
			_faction = faction;
			SetStuffDef();
		}

		public override IEnumerable<string> ConfigErrors(TraderKindDef parentDef)
		{
			if (_stuffDef == null)
			{
				SetStuffDef();
			}

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
		/// Sell anything which could potentially be made from the chosen stuff.
		/// </summary>
		/// <param name="def">Thing to check</param>
		/// <param name="forTile">Tile in which the transaction takes place.</param>
		/// <param name="faction">Faction of the trader.</param>
		/// <returns>If the item can be sold or not.</returns>
		protected override bool CanSell(in ThingDef def, in int forTile, in Faction faction)
		{
			// Only accept things which could be made from the chosen stuff.
			return _stuffDef.IsStuff && _stuffDef.stuffProps.CanMake(def) &&
			       // Only sell non-armor apparel, furniture or art.
			       (def.IsApparel && !Util.IsArmor(def) || def.Minifiable &&
				       (def.IsWithinCategory(DefOf.ThingCategory.BuildingsFurniture) ||
				        def.IsWithinCategory(ThingCategoryDefOf.BuildingsArt) ||
				        def.IsWithinCategory(DefOf.ThingCategory.BuildingsJoy)));
		}

		/// <summary>
		/// Purchases the chosen stuff and anything which could potentially be made from the same stuff.
		/// Weapons are handled in other StockGens.
		/// </summary>
		/// <param name="def">ThingDef being checked.</param>
		/// <returns>True if the trader will buy the item.</returns>
		protected override bool CanBuy(in ThingDef def)
		{
			return def == _stuffDef || CanSell(def, -1, null);
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
			return def.IsApparel
				? StockGenerator_Clothes.SelectionWeightMarketValueCurve.Evaluate(def.BaseMarketValue)
				: StockGenerator_Art.SelectionWeightMarketValueCurve.Evaluate(def.BaseMarketValue);
		}
	}
}