using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace TraderGen.StockGen
{
	/// <summary>
	/// Handles anything with a high enough market value per unit of volume.
	/// Intended to replace StockGenerator_BuyExpensiveSimple. It ignores additional objects such as organs, eggs or
	/// relics. If needed it can also be used to generate random, high-value stock.
	/// </summary>
	public class ExpensiveSimple : ConditionMatcher
	{
		/// <summary>
		/// Minimum value per unit of volume. The trader will buy anything above this threshold.
		/// </summary>
		public float minValuePerUnit = 10f;

		/// <summary>
		/// Maximum value per unit of volume. Will only generate stock between minValuePerUnit and maxValuePerUnit.
		/// </summary>
		public float maxValuePerUnit = float.MaxValue;

		/// <summary>
		/// Lazily initialized cache of things disallowed from purchase. Initialized in CanBuy.
		/// </summary>
		private static HashSet<ThingDef> _disallowedPurchase;

		/// <summary>
		/// Lazily initialized cache of things disallowed from sale.. Initialized in CanSell.
		/// </summary>
		private static HashSet<ThingDef> _disallowedSale;

		public override void ConditionToText(ref StringBuilder b)
		{
			base.ConditionToText(ref b);
			b.Append($"minValuePerUnit: {minValuePerUnit}\n");
			if (maxValuePerUnit < float.MaxValue)
			{
				b.Append($"maxValuePerUnit: {maxValuePerUnit}\n");
			}
		}

		private static float ValuePerUnit(in ThingDef def)
		{
			return def.BaseMarketValue / def.VolumePerUnit;
		}

		protected override bool CanSell(in ThingDef def, in int forTile, in Faction faction)
		{
			if (_disallowedSale == null)
			{
				_disallowedSale = new HashSet<ThingDef>();
				_disallowedSale.AddRange(DefOf.ThingCategory.BodyParts.DescendantThingDefs);
				if (DefOf.ThingCategory.GR_GeneticMaterial != null)
				{
					_disallowedSale.AddRange(DefOf.ThingCategory.GR_GeneticMaterial.DescendantThingDefs);
				}

				if (ModsConfig.IdeologyActive)
				{
					_disallowedSale.Add(DefOf.Thing.GauranlenSeed);
				}
			}

			return !_disallowedSale.Contains(def) && ValuePerUnit(def) <= maxValuePerUnit && base.CanSell(def, forTile, faction);
		}

		protected override bool CanBuy(in ThingDef def)
		{
			if (_disallowedPurchase == null)
			{
				_disallowedPurchase = new HashSet<ThingDef>();
				_disallowedPurchase.AddRange(DefOf.ThingCategory.BodyPartsNatural.DescendantThingDefs);
				_disallowedPurchase.AddRange(DefOf.ThingCategory.EggsFertilized.DescendantThingDefs);
				_disallowedPurchase.AddRange(DefOf.ThingCategory.EggsUnfertilized.DescendantThingDefs);
				_disallowedPurchase.AddRange(DefOf.ThingCategory.InertRelics.DescendantThingDefs);
			}

			return def.category == ThingCategory.Item && !def.IsApparel && !def.IsWeapon &&
			       !_disallowedPurchase.Contains(def) && ValuePerUnit(def) >= minValuePerUnit;
		}
	}
}