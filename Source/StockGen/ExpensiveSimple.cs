using System.Collections.Generic;
using System.Text;
using RimWorld;
using Verse;
using Thing = TraderGen.DefOfs.Thing;

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
		/// Lazily initialized cache of things disallowed from sale. Initialized in CanSell.
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
				_disallowedSale.AddRange(DefOfs.ThingCategory.BodyParts.DescendantThingDefs);
				if (ModsConfig.IdeologyActive)
				{
					_disallowedSale.Add(Thing.GauranlenSeed);
					_disallowedSale.Add(ThingDefOf.Skull);
				}

				if (ModsConfig.BiotechActive)
				{
					_disallowedSale.Add(ThingDefOf.Xenogerm);
					_disallowedSale.Add(ThingDefOf.Genepack);
					_disallowedSale.Add(ThingDefOf.HumanEmbryo);
					_disallowedSale.Add(ThingDefOf.HumanOvum);
					_disallowedSale.Add(Thing.SignalChip);
					_disallowedSale.Add(Thing.PowerfocusChip);
					_disallowedSale.Add(Thing.NanostructuringChip);
				}

				// These defs are active if both Alpha Memes and Biotech are active at the same time.
				if (DefDatabase<ThingDef>.defsByName.TryGetValue("AM_HyperLinkageChip", out ThingDef hyperLinkageChip))
				{
					_disallowedSale.Add(hyperLinkageChip);
					_disallowedSale.Add(DefDatabase<ThingDef>.GetNamedSilentFail("AM_StellarProcessingChip"));
					_disallowedSale.Add(DefDatabase<ThingDef>.GetNamedSilentFail("AM_QuantumMatrixChip"));
				}

				if (Thing.AG_Alphapack != null)
				{
					_disallowedSale.Add(Thing.AG_Alphapack);
					_disallowedSale.Add(Thing.AG_Mixedpack);
				}

				if (DefOfs.ThingCategory.GR_GeneticMaterial != null)
				{
					_disallowedSale.AddRange(DefOfs.ThingCategory.GR_GeneticMaterial.DescendantThingDefs);
				}
			}

			return !_disallowedSale.Contains(def) && ValuePerUnit(def) <= maxValuePerUnit &&
			       base.CanSell(def, forTile, faction);
		}

		protected override bool CanBuy(in ThingDef def)
		{
			if (_disallowedPurchase == null)
			{
				_disallowedPurchase = new HashSet<ThingDef>();
				_disallowedPurchase.AddRange(DefOfs.ThingCategory.BodyPartsNatural.DescendantThingDefs);
				_disallowedPurchase.AddRange(DefOfs.ThingCategory.EggsFertilized.DescendantThingDefs);
				_disallowedPurchase.AddRange(DefOfs.ThingCategory.EggsUnfertilized.DescendantThingDefs);
				_disallowedPurchase.AddRange(DefOfs.ThingCategory.InertRelics.DescendantThingDefs);
			}

			return def.category == ThingCategory.Item && !def.IsApparel && !def.IsWeapon &&
			       !_disallowedPurchase.Contains(def) && ValuePerUnit(def) >= minValuePerUnit;
		}
	}
}