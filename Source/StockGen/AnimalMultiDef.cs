using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace TraderGen.StockGen
{
	/// <summary>
	/// Sells kindCountRange random types of animals in pawnKindDefs, with countRange/totalPriceRange in stock for each.
	/// Uses the same random weight depending on wildness as StockGenerator_Animals.
	/// Takes into account totalPriceRange and customCountRanges, which are ignored by vanilla StockGenerator_Animals.
	/// Includes an option to not generate animals in unsafe temperatures.
	/// Also has an option for generating animal products of the chosen animals.
	/// </summary>
	public class AnimalMultiDef : StockGen
	{
		public List<PawnKindDef> pawnKindDefs = new List<PawnKindDef>();

		public IntRange kindCountRange = new IntRange(1, 1);

		public bool checkTemperature = false;

		public bool newborn = false;

		public bool animalProducts = false;

		public FloatRange animalProductPriceRange = FloatRange.Zero;

		private HashSet<ThingDef> _cachedAnimals;

		private Dictionary<PawnKindDef, HashSet<ThingDef>> _cachedAnimalProducts;

		private const string VfeAnimalProductComp = "AnimalBehaviours.CompProperties_AnimalProduct";
		private static FieldInfo _vfeResourceDef;
		private static FieldInfo _vfeProductIsRandom;
		private static FieldInfo _vfeProductSeasonal;

		/// <summary>
		/// Obtains animal products defined using the VEF comp through reflection to avoid a dependency.
		/// Items with ExoticMisc tradeTags are ignored.
		/// Animals with random or seasonal animal products are also ignored.
		/// </summary>
		/// <param name="comp">AnimalBehaviours.CompProperties_AnimalProduct instance</param>
		/// <returns>Animal product found, if any.</returns>
		private static ThingDef VefAnimalProduct(CompProperties comp)
		{
			if (_vfeResourceDef == null)
			{
				var compType = AccessTools.TypeByName(VfeAnimalProductComp);
				_vfeResourceDef = compType.GetField("resourceDef", BindingFlags.Public | BindingFlags.Instance);
				_vfeProductIsRandom = compType.GetField("isRandom", BindingFlags.Public | BindingFlags.Instance);
				_vfeProductSeasonal = compType.GetField("seasonalItems", BindingFlags.Public | BindingFlags.Instance);
				if (_vfeResourceDef == null || _vfeProductIsRandom == null || _vfeProductSeasonal == null)
				{
					Logger.Error($"Could not load fields of {VfeAnimalProductComp}.");
					return null;
				}
			}

			var productDef = (ThingDef)_vfeResourceDef.GetValue(comp);
			var isRandom = (bool)_vfeProductIsRandom.GetValue(comp);
			var seasonal = (List<string>)_vfeProductSeasonal.GetValue(comp);

			if (productDef == null || isRandom || !seasonal.NullOrEmpty())
			{
				return null;
			}

			return productDef;
		}

		private void UpdateCache()
		{
			if (_cachedAnimals != null)
			{
				return;
			}

			_cachedAnimals = new HashSet<ThingDef>();
			foreach (PawnKindDef pawnKindDef in pawnKindDefs)
			{
				_cachedAnimals.Add(pawnKindDef.race);
			}

			_cachedAnimalProducts = new Dictionary<PawnKindDef, HashSet<ThingDef>>();
			if (!animalProducts || animalProductPriceRange == FloatRange.Zero)
			{
				return;
			}

			foreach (PawnKindDef pawnKindDef in pawnKindDefs)
			{
				_cachedAnimalProducts[pawnKindDef] = new HashSet<ThingDef>();
				foreach (CompProperties compProperties in pawnKindDef.race.comps)
				{
					List<ThingDef> animalProductDefs = new List<ThingDef>();
					switch (compProperties)
					{
						case CompProperties_Shearable shearableComp:
							if (shearableComp.woolDef != null)
							{
								animalProductDefs.Add(shearableComp.woolDef);
							}

							break;
						case CompProperties_Milkable milkableComp:
							if (milkableComp.milkDef != null)
							{
								animalProductDefs.Add(milkableComp.milkDef);
							}

							break;
						case CompProperties_EggLayer eggComp:
							if (eggComp.eggUnfertilizedDef != null)
							{
								animalProductDefs.Add(eggComp.eggUnfertilizedDef);
							}

							break;
						default:
						{
							if (compProperties.GetType().FullName == VfeAnimalProductComp)
							{
								ThingDef product = VefAnimalProduct(compProperties);
								if (product != null)
								{
									animalProductDefs.Add(product);
								}
							}

							break;
						}
					}

					foreach (ThingDef animalProductDef in animalProductDefs)
					{
						if (animalProductDef.tradeability.TraderCanSell() && !Things.Util.IsExotic(animalProductDef))
						{
							_cachedAnimalProducts[pawnKindDef].Add(animalProductDef);
						}
					}
				}
			}
		}

		public override IEnumerable<string> ConfigErrors(TraderKindDef parentDef)
		{
			foreach (var err in base.ConfigErrors(parentDef))
			{
				yield return err;
			}

			foreach (PawnKindDef pawnKindDef in pawnKindDefs)
			{
				if (pawnKindDef.race?.race == null || !pawnKindDef.race.race.Animal)
				{
					yield return $"TraderGen.StockGen.AnimalMultiDef: {pawnKindDef} is not an animal.";
				}
			}
		}

		public override void ToText(ref StringBuilder b)
		{
			b.Append($"pawnKindDefs: {string.Join(", ", pawnKindDefs)}\n");

			b.Append($"kindCountRange: {kindCountRange}\n");

			if (checkTemperature)
			{
				b.Append($"checkTemperature: {checkTemperature}\n");
			}

			if (newborn)
			{
				b.Append($"newborn: {newborn}\n");
			}

			if (animalProducts)
			{
				b.Append($"animalProducts: {animalProducts}\n");
			}

			if (animalProductPriceRange != FloatRange.Zero)
			{
				b.Append($"animalProductPriceRange: {animalProductPriceRange}\n");
			}
		}

		/// <summary>
		/// Returns the number of stock that should be generated for a given animal.
		/// </summary>
		/// <param name="pawnDef">Animal to check</param>
		/// <returns>Animals of this species in stock.</returns>
		protected virtual int AnimalCount(PawnKindDef pawnDef)
		{
			return RandomCountOf(pawnDef.race);
		}

		/// <summary>
		/// Filters out any animals that could not survive the temperature on this tile.
		/// </summary>
		/// <param name="forTile">Tile in which the transaction takes place.</param>
		/// <param name="faction">Faction of the trader.</param>
		/// <returns>Filtered out list of animals.</returns>
		private List<PawnKindDef> AcceptablePawnKindDefs(int forTile, Faction faction = null)
		{
			if (!checkTemperature)
			{
				return pawnKindDefs;
			}

			var tempTile = forTile;
			if (tempTile == -1 && Find.AnyPlayerHomeMap != null)
			{
				tempTile = Find.AnyPlayerHomeMap.Tile;
			}

			if (tempTile != -1)
			{
				return pawnKindDefs.Where(def => Find.World.tileTemperatures.SeasonAndOutdoorTemperatureAcceptableFor(
					tempTile,
					def.race)).ToList();
			}

			return pawnKindDefs;
		}

		public override IEnumerable<Thing> GenerateThings(int forTile, Faction faction = null)
		{
			UpdateCache();

			var chosenPawnDefs = Algorithm.ChooseNWeightedRandomly(AcceptablePawnKindDefs(forTile),
				def => StockGenerator_Animals.SelectionChanceFromWildnessCurve.Evaluate(def.RaceProps.wildness),
				kindCountRange.RandomInRange);

			foreach (PawnKindDef pawnKindDef in chosenPawnDefs)
			{
				var count = AnimalCount(pawnKindDef);
				for (int animalIndex = 0; animalIndex < count; ++animalIndex)
				{
					PawnGenerationRequest pawnGenerationRequest = new PawnGenerationRequest(pawnKindDef, tile: forTile);
					if (newborn)
					{
						pawnGenerationRequest.AllowedDevelopmentalStages = DevelopmentalStage.Newborn;
					}

					yield return PawnGenerator.GeneratePawn(pawnGenerationRequest);
				}

				if (!animalProducts || animalProductPriceRange == FloatRange.Zero)
				{
					continue;
				}

				// Generate stock for animal products of the chosen animal.
				foreach (ThingDef productThingDef in _cachedAnimalProducts[pawnKindDef])
				{
					if (!productThingDef.tradeability.TraderCanSell() || Things.Util.IsExotic(productThingDef) ||
					    TraderKind.Cache.WillNotStock(TraderKind.Cache.GenerationSeed, productThingDef))
					{
						continue;
					}

					int productCount = Mathf.RoundToInt(animalProductPriceRange.RandomInRange / productThingDef.BaseMarketValue);
					foreach (Thing thing in StockGeneratorUtility.TryMakeForStock(productThingDef, productCount, faction))
					{
						yield return thing;
					}
				}
			}
		}

		public override bool HandlesThingDef(ThingDef thingDef)
		{
			UpdateCache();
			if (_cachedAnimals.Contains(thingDef))
			{
				return true;
			}

			foreach (HashSet<ThingDef> productSet in _cachedAnimalProducts.Values)
			{
				if (productSet.Contains(thingDef))
				{
					return true;
				}
			}

			return false;
		}
	}
}