using System.Collections.Generic;
using System.Linq;
using Force.DeepCloner;
using RimWorld;
using TG.Mod;
using TG.StockGen;
using Verse;

namespace TG.TraderKind
{
	public static class Generator
	{
		/// <summary>
		/// Creates a copy with initialized internal state if necessary. Logs stock generator information if enabled.
		/// </summary>
		/// <param name="gen">StockGenerator being added</param>
		/// <param name="tile">Map tile in which the transaction takes place.</param>
		/// <param name="faction">Faction of the trader.</param>
		/// <returns></returns>
		private static StockGenerator StockGenFrom(StockGenerator gen, int tile, Faction faction)
		{
			var generator = gen;
			// This code assumes that vanilla StockGenerators have no internal state requiring a copy.
			if (gen.GetType().IsSubclassOf(typeof(TG.StockGen.StockGen)))
			{
				generator = gen.ShallowClone();
				((TG.StockGen.StockGen) generator).BeforeGen(tile, faction);
			}

			if (Settings.LogGen && Settings.LogStockGen)
			{
				Logger.Gen(Util.ToText(generator).ToString());
			}

			return generator;
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
		/// <param name="specializations">Specializations chosen for this trader.</param>
		/// <returns>New TraderKindDef. The caller is responsible for managing its life cycle.</returns>
		public static TraderKindDef Def(TraderKindDef originalDef, int seed, int tile, Faction faction,
			out List<TraderSpecializationDef> specializations)
		{
			Logger.Gen($"Generating stock for {originalDef.defName}.");
			var def = originalDef.ShallowClone();
			def.generated = true;
			def.stockGenerators = new List<StockGenerator>();

			Rand.PushState(seed);
			// Add stock generators from the template TraderKindDef.
			foreach (var gen in originalDef.stockGenerators)
			{
				def.stockGenerators.Add(StockGenFrom(gen, tile, faction));
			}

			// Stock generators coming from the specialization(s).
			specializations = new List<TraderSpecializationDef>();
			var specializationNames = new List<string>();
			var extension = originalDef.GetModExtension<GenExtension>();
			if (extension != null)
			{
				// ToDo configurable amount of specializations.
				var chosenSpecializations =
					Algorithm.ChooseNWeightedRandomly(extension.specializations, spec => spec.commonality, 1);
				foreach (var specialization in chosenSpecializations)
				{
					Logger.Gen($"Adding specialization {specialization.def.defName}");
					specializations.Add(specialization.def);
					specializationNames.Add(specialization.def.label);

					foreach (var gen in specialization.def.stockGens)
					{
						def.stockGenerators.Add(StockGenFrom(gen, tile, faction));
					}
				}
			}

			Rand.PopState();

			if (specializationNames.Count > 0)
			{
				def.label = $"{def.label} ({string.Join(", ", specializationNames)})";
			}

			return def;
		}

		public static string Name(TraderKindDef def, Faction faction, List<TraderSpecializationDef> specializations)
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