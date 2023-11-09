using System.Collections.Generic;
using System.Linq;
using RimWorld;
using TraderGen.DefOfs;
using Verse;
using ThingCategory = Verse.ThingCategory;

namespace TraderGen.Things
{
	/// <summary>
	/// Utility functions for dealing with Things and ThingDefs
	/// </summary>
	public static class Util
	{
		/// <summary>
		/// Caches ThingDefs associated to a HediffDef.
		/// </summary>
		private static Dictionary<ThingDef, HediffDef> _hediffDefOf;


		/// <summary>
		/// Body mod which is not created artificially.
		/// </summary>
		private static HashSet<ThingDef> _naturalBodyMod;

		/// <summary>
		/// Returns true if the provided def should be considered armor when generating trader stock.
		/// </summary>
		/// <param name="def">Provided ThingDef</param>
		/// <returns>True if the apparel is armor.</returns>
		public static bool IsArmor(in ThingDef def)
		{
			if (!def.IsApparel)
			{
				return false;
			}

			// Armor rating stat threshold that must be reached before an apparel is considered armor.
			// See RimWorld.StockGenerator_Clothes.
			const float armorThreshold = 0.15f;

			var stuff = GenStuff.DefaultStuffFor(def);
			return def.GetStatValueAbstract(StatDefOf.ArmorRating_Blunt, stuff) > armorThreshold ||
			       def.GetStatValueAbstract(StatDefOf.ArmorRating_Sharp, stuff) > armorThreshold;
		}

		/// <summary>
		/// Checks if provided def is an alcoholic drink.
		/// </summary>
		/// <param name="def">Provided ThingDef</param>
		/// <returns>True if def is an alcoholic drink.</returns>
		public static bool IsAlcohol(in ThingDef def)
		{
			if (def.ingestible?.outcomeDoers == null || def.ingestible.JoyKind != JoyKind.Chemical ||
			    !def.ingestible.foodType.HasFlag(FoodTypeFlags.Liquor)) return false;

			return (from outcomeDoer in def.ingestible.outcomeDoers
				where outcomeDoer.GetType() == typeof(IngestionOutcomeDoer_GiveHediff)
				select (IngestionOutcomeDoer_GiveHediff) outcomeDoer).Any(o =>
				o.hediffDef?.hediffClass == typeof(Hediff_Alcohol));
		}

		/// <summary>
		/// FoodTypeFlags for ingredients acceptable for vegans.
		/// </summary>
		private static readonly FoodTypeFlags[] VeganFlags = {FoodTypeFlags.VegetableOrFruit, FoodTypeFlags.Seed};

		/// <summary>
		/// Checks if the def is a type of raw vegan food. Ignores fungus as that is handled by other precept.
		/// </summary>
		/// <param name="def">Provided ThingDef</param>
		/// <returns>True for all raw vegan foods.</returns>
		public static bool IsRawVegan(ThingDef def)
		{
			return def.IsIngestible && VeganFlags.Any(flag => def.ingestible.foodType.HasFlag(flag));
		}

		/// <summary>
		/// Checks if the def is a type of meat which is not human or insect meat.
		/// </summary>
		/// <param name="def">Provided ThingDef</param>
		/// <returns>True for all meats.</returns>
		public static bool IsRegularMeat(in ThingDef def)
		{
			return def.IsIngestible && FoodUtility.GetMeatSourceCategory(def) == MeatSourceCategory.Undefined;
		}

		/// <summary>
		/// Returns true if the provided def is a stuff of the woody category.
		/// </summary>
		/// <param name="def">Provided ThingDef</param>
		/// <returns>True if the stuff is a type of wood.</returns>
		public static bool IsWoodyStuff(in ThingDef def)
		{
			return def.stuffProps?.categories != null && def.stuffProps.categories.Contains(StuffCategoryDefOf.Woody);
		}

		/// <summary>
		/// Check if a thing is an exotic item.
		/// </summary>
		/// <param name="def">Thing to check.</param>
		/// <returns>If the thing is an exotic misc item or not.</returns>
		public static bool IsExotic(in ThingDef def)
		{
			// Some mods use "Exotic" instead of "ExoticMisc".
			return def.tradeTags != null && (def.tradeTags.Contains("ExoticMisc") || def.tradeTags.Contains("Exotic"));
		}

		/// <summary>
		/// False for animals which cannot be tamed, all genetic animals from VGE, and awakened dryads.
		/// Used to prevent using venerated animals as a loophole to obtain them.
		/// </summary>
		/// <param name="def">Pawn being checked.</param>
		/// <returns>True if a trader could have this animal in stock.</returns>
		public static bool ObtainableAnimal(in PawnKindDef def)
		{
			if (def.race?.comps != null)
			{
				foreach (var comp in def.race.comps)
				{
					switch (comp.GetType().FullName)
					{
						// Mech hybrids in Vanilla Genetics Expanded have this comp.
						case "GeneticRim.CompProperties_RegisterMechHybridWithAntenna":
							return false;
						// Untameable animals use this Vanilla Expanded Framework comp.
						case "AnimalBehaviours.CompProperties_Untameable":
							return false;
					}
				}
			}

			// Prevent awakened dryads.
			if (def.race?.tradeTags != null && def.race.tradeTags.Contains("VDE_Dryad"))
			{
				return false;
			}

			// Hybrids and paragons in Vanilla Genetics Expanded use this mod extension.
			return def.modExtensions == null ||
			       def.modExtensions.All(extension => extension.GetType().FullName != "GeneticRim.DefExtension_Hybrid");
		}

		/// <summary>
		/// Caches ThingDefs that are spawned after a HediffDef is removed.
		/// </summary>
		private static void InitializeHediffDefOf()
		{
			if (_hediffDefOf != null) return;

			_hediffDefOf = new Dictionary<ThingDef, HediffDef>();

			foreach (var hediffDef in DefDatabase<HediffDef>.AllDefsListForReading)
			{
				if (hediffDef.spawnThingOnRemoved != null &&
				    DefOfs.ThingCategory.BodyParts.DescendantThingDefs.Contains(hediffDef.spawnThingOnRemoved))
				{
					_hediffDefOf[hediffDef.spawnThingOnRemoved] = hediffDef;
				}
			}
		}

		/// <summary>
		/// A body mod is a thing that spawns after removing a hediff that counts as an implant.
		/// </summary>
		/// <param name="def">ThingDef being checked.</param>
		/// <returns>True if it is a body mod.</returns>
		public static bool IsBodyMod(in ThingDef def)
		{
			InitializeHediffDefOf();
			// Although technically wood logs are implants,body purists should make an exception and still purchase it.
			return def != ThingDefOf.WoodLog && _hediffDefOf.ContainsKey(def) && _hediffDefOf[def].countsAsAddedPartOrImplant;
		}

		private static void InitializeNaturalBodyMods()
		{
			if (_naturalBodyMod != null) return;

			// Gather all natural body parts.
			_naturalBodyMod = DefOfs.ThingCategory.BodyPartsNatural.DescendantThingDefs.ToHashSet();
			if (DefOfs.ThingCategory.AA_ImplantCategory != null)
			{
				_naturalBodyMod.UnionWith(DefOfs.ThingCategory.AA_ImplantCategory.DescendantThingDefs);
			}

			if (DefOfs.ThingCategory.VFEI_BodyPartsInsect != null)
			{
				_naturalBodyMod.UnionWith(DefOfs.ThingCategory.VFEI_BodyPartsInsect.DescendantThingDefs);
			}

			if (DefOfs.ThingCategory.GR_ImplantCategory != null)
			{
				_naturalBodyMod.UnionWith(DefOfs.ThingCategory.GR_ImplantCategory.DescendantThingDefs);
			}

			// Keep only those that count as an added part or implant.
			_naturalBodyMod.IntersectWith(_hediffDefOf.Keys);
		}

		/// <summary>
		/// Thing that counts as an artificial body mod. See ThoughtWorker_Precept_HasNonNaturalProsthetic in VIE-MS.
		/// </summary>
		/// <param name="def">ThingDef being checked.</param>
		/// <returns>True if it is an artificial body mod.</returns>
		public static bool IsArtificialBodyMod(in ThingDef def)
		{
			if (!IsBodyMod(def))
			{
				return false;
			}

			InitializeNaturalBodyMods();

			return IsBodyMod(def) && !_naturalBodyMod.Contains(def);
		}

		/// <summary>
		/// Checks if a thing should be considered a drug. Alcohol is counted as a drug in this check.
		/// </summary>
		/// <param name="def">ThingDef being checked.</param>
		/// <returns>True if the item is any kind of drug.</returns>
		public static bool IsDrug(in ThingDef def)
		{
			return def.IsIngestible && def.IsWithinCategory(ThingCategoryDefOf.Drugs) && (def.thingCategories == null ||
				!def.thingCategories.Any(cat => cat.defName == "VBE_DrinksNonAlcoholic"));
		}
	}
}