using System;
using System.Collections.Generic;
using Force.DeepCloner;
using RimWorld;
using TG.Mod;
using Verse;

namespace TG.TraderKind
{
	public static class Generator
	{
		/// <summary>
		/// Creates a copy with initialized internal state if necessary. Logs stock generator information if enabled.
		/// </summary>
		/// <param name="def">Owner of the stock generator.</param>
		/// <param name="gen">StockGenerator being added.</param>
		/// <param name="tile">Map tile in which the transaction takes place.</param>
		/// <param name="faction">Faction of the trader.</param>
		/// <returns></returns>
		private static StockGenerator StockGenFrom(TraderKindDef def, StockGenerator gen, int tile, Faction faction)
		{
			var generator = gen.ShallowClone();
			generator.ResolveReferences(def);

			// This code assumes that vanilla StockGenerators have no internal state requiring a copy.
			if (gen.GetType().IsSubclassOf(typeof(TG.StockGen.StockGen)))
			{
				((TG.StockGen.StockGen) generator).BeforeGen(tile, faction);
			}

			if (Settings.LogGen && Settings.LogStockGen)
			{
				Logger.Gen(StockGen.Util.ToText(generator).ToString());
			}

			return generator;
		}

		/// <summary>
		/// Apply specializations to the trader, if it has any.
		/// </summary>
		/// <param name="originalDef">Template trader.</param>
		/// <param name="def">New trader created from the template.</param>
		/// <param name="tile">Map tile in which the transaction takes place.</param>
		/// <param name="faction">Faction of the trader.</param>
		private static void ApplySpecializations(TraderKindDef originalDef, ref TraderKindDef def, int tile,
			Faction faction)
		{
			var extension = originalDef.GetModExtension<GenExtension>();

			if (extension == null)
			{
				return;
			}

			var numSpecializations = Math.Max(Settings.OrbitalSpecializations.RandomInRange, 0);

			if (numSpecializations <= 0)
			{
				return;
			}

			var chosenSpecializations =
				Algorithm.ChooseNWeightedRandomly(extension.specializations, spec => spec.commonality,
					numSpecializations);

			var specializationNames = new List<string>();
			foreach (var specialization in chosenSpecializations)
			{
				Logger.Gen($"Adding specialization {specialization.def.defName}");
				specializationNames.Add(specialization.def.label);

				foreach (var gen in specialization.def.stockGens)
				{
					def.stockGenerators.Add(StockGenFrom(def, gen, tile, faction));
				}
			}

			if (specializationNames.Count > 0)
			{
				def.label = $"{def.label} ({string.Join(", ", specializationNames)})";
			}
		}

		/// <summary>
		/// Generate a new TraderKindDef from a template.
		/// StockGenerators with randomized internal state will be correctly initialized.
		/// Specializations will be added, if the TraderKindDef has a GenExtension defining them.
		/// The new TraderKindDef is not registered into the DefDatabase so it should not be used by the game in any way.
		/// Since this def is not registered anywhere, it will be garbage-collected if the caller stops using it.
		/// </summary>
		/// <param name="originalDef">Template TraderKindDef</param>
		/// <param name="seed">Random seed to use for generation.</param>
		/// <param name="tile">Map tile in which the transaction takes place.</param>
		/// <param name="faction">Faction of the trader.</param>
		/// <returns>New TraderKindDef. The caller is responsible for managing its life cycle.</returns>
		public static TraderKindDef Def(TraderKindDef originalDef, int seed, int tile, Faction faction)
		{
			Logger.Gen($"Generating stock for {originalDef.defName}.");
			var def = originalDef.ShallowClone();
			def.generated = true;
			def.stockGenerators = new List<StockGenerator>();

			Rand.PushState(seed);
			// Add stock generators from the template TraderKindDef.
			foreach (var gen in originalDef.stockGenerators)
			{
				def.stockGenerators.Add(StockGenFrom(def, gen, tile, faction));
			}

			ApplySpecializations(originalDef, ref def, tile, faction);

			Rand.PopState();

			return def;
		}

		public static string Name(TraderKindDef def, Faction faction)
		{
			var extension = def.GetModExtension<GenExtension>();
			if (extension?.extraNameRules == null) return null;
			var name = NameGenerator.GenerateName(extension.extraNameRules);

			if (faction != null)
			{
				name = $"{name} {"OfLower".Translate()} {faction.name}";
			}

			return name;
		}
	}
}