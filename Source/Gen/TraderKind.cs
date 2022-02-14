using Force.DeepCloner;
using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace TG.Gen
{
	/// <summary>
	/// Stores information required to recreate a TraderKindDef if necessary.
	/// </summary>
	public class TraderGenData : IExposable
	{
		/// <summary>
		/// Procedural generation template used to create the TraderKindDef.
		/// </summary>
		public TraderGenDef Def;

		/// <summary>
		/// Seed used to generate the TraderKindDef using the TraderGenDef.
		/// </summary>
		public int Seed;

		public void ExposeData()
		{
			Scribe_Defs.Look(ref Def, "Def");
			Scribe_Values.Look(ref Seed, "Seed");
		}
	}

	/// <summary>
	/// Procedurally generates TraderKindDefs and also manages their life cycle, save and load.
	/// </summary>
	public class TraderKind : WorldComponent
	{
		/// <summary>
		/// Maximum Markov chain depth allowed.
		/// </summary>
		private const int MaxDepth = 32;

		private Dictionary<string, TraderGenData> _defReferencesByName;

		public TraderKind(World world) : base(world)
		{
			_defReferencesByName = new Dictionary<string, TraderGenData>();
		}

		/// <summary>
		/// Adds all data found in a LinkDef to the TraderKindDef being generated
		/// </summary>
		/// <param name="def">TraderKindDef currently being generated</param>
		/// <param name="linkDef">LinkDef in which the chain is currently on.</param>
		/// <param name="depth">Depth of the current Markov chain.</param>
		private static void ProcessLink(ref TraderKindDef def, in LinkDef linkDef, in int depth)
		{
			if (linkDef.stockGroups != null)
			{
				foreach (var stockGroup in linkDef.stockGroups)
				{
					Logger.Gen($"{depth}: Processing StockGroup {stockGroup}.");

					// StockGenerators have a reference to their TraderKindDef. This reference is not set for generators coming
					// from a StockGroupDef as these lack a TraderKindDef. Since multiple TraderKindDefs may be using the same
					// StockGroupDef at any given time, a shallow copy of the StockGenerator is provided to them instead.
					// Each copy can then point to the right TraderKindDef, and will be deleted when its TraderKindDef is deleted.
					foreach (var stockGeneratorCopy in stockGroup.generators.Select(stockGenerator =>
						         stockGenerator.ShallowClone()))
					{
						def.stockGenerators.Add(stockGeneratorCopy);
						stockGeneratorCopy.ResolveReferences(def);
						Logger.Gen(
							$"{depth}: Adding StockGenerator {Logger.StockGen(stockGeneratorCopy)}");
					}
				}
			}
		}

		/// <summary>
		/// Visits a Markov chain link, processes it, and chooses the next link to explore (if any).
		/// </summary>
		/// <param name="rng"></param>
		/// <param name="def">TraderKindDef currently being generated</param>
		/// <param name="linkDef">LinkDef in which the chain is currently on.</param>
		/// <param name="depth">Depth of the current Markov chain.</param>
		private static void VisitLink(ref Random rng, ref TraderKindDef def, in LinkDef linkDef, int depth = 1)
		{
			Logger.Gen($"{depth}: Arrived at link {linkDef.defName}.");
			if (depth >= MaxDepth)
			{
				Logger.Error($"Maximum depth {MaxDepth} reached. No further links will be followed.");
				return;
			}

			ProcessLink(ref def, linkDef, depth);

			var totalCommonality = linkDef.TotalCommonality;
			if (totalCommonality <= 0)
			{
				return;
			}

			LinkDef chosenLinkDef;
			if (totalCommonality == 1)
			{
				chosenLinkDef = linkDef.next.First().def;
			}
			else
			{
				// Next uses an exclusive upper bound.
				var chosenCommonality = rng.Next(0, totalCommonality + 1);
				var currentCommonality = 0;
				var index = 0;

				for (; index < linkDef.next.Count; ++index)
				{
					currentCommonality += linkDef.next[index].commonality;
					if (currentCommonality >= chosenCommonality)
					{
						break;
					}
				}

				chosenLinkDef = linkDef.next[index].def;
			}

			VisitLink(ref rng, ref def, chosenLinkDef, depth + 1);
		}

		/// <summary>
		/// Procedurally generates a new TraderKindDef if necessary, and registers it into DefDatabase.
		/// </summary>
		/// <param name="data">Trader generation preset and seed to use.</param>
		/// <returns>Newly generated TraderKindDef.</returns>
		public TraderKindDef GenWithSeed(in TraderGenData data)
		{
			var newDefName = data.Def.defName + '_' + data.Seed;

			// The def may exist already after loading without quitting RimWorld.
			DefDatabase<TraderKindDef>.defsByName.TryGetValue(newDefName, out var def);
			if (def != null)
			{
				Logger.Gen($"TraderKindDef {newDefName} already exists, generation unnecessary.");
				return def;
			}

			def = new TraderKindDef
			{
				defName = newDefName,
				label = data.Def.label,
				modExtensions = data.Def.modExtensions,
				modContentPack = data.Def.modContentPack,
				fileName = data.Def.fileName,
				generated = true,
				orbital = data.Def.orbital,
				requestable = data.Def.requestable,
				hideThingsNotWillingToTrade = data.Def.hideThingsNotWillingToTrade,
				tradeCurrency = data.Def.tradeCurrency,
				faction = data.Def.faction,
				permitRequiredForTrading = data.Def.permitRequiredForTrading
			};

			Logger.Gen($"Generating new TraderKindDef {def.defName} using seed {data.Seed}.");

			var rng = new Random(data.Seed);

			foreach (var link in data.Def.links)
			{
				Logger.Gen("Starting new chain.");
				VisitLink(ref rng, ref def, link);
			}

			Logger.Gen("Final generators:");
			foreach (var generator in def.stockGenerators)
			{
				Logger.Gen($"\t{Logger.StockGen(generator)}");
			}

			def.PostLoad();

			// _defReferencesByName will already have a newDefName entry when called from ExposeData.
			_defReferencesByName.TryAdd(newDefName, data);
			ShortHashGiver.GiveShortHash(def, def.GetType());
			DefDatabase<TraderKindDef>.Add(def);
			Logger.Gen($"Finished generating new TraderKindDef {def.defName}.");

			return def;
		}

		public TraderKindDef Generate(in TraderGenDef genDef)
		{
			return GenWithSeed(new TraderGenData
			{
				Def = genDef,
				Seed = Math.Abs(Rand.Int)
			});
		}

		public void Remove(in TraderKindDef def)
		{
			var defName = def.defName;
			DefDatabase<TraderKindDef>.Remove(def);
			ShortHashGiver.takenHashesPerDeftype[def.GetType()].Remove(def.shortHash);
			_defReferencesByName.Remove(def.defName);
			Logger.Gen($"Removed previously generated TraderKindDef {defName}.");
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Collections.Look(ref _defReferencesByName, "_defReferencesByName", LookMode.Value, LookMode.Deep);
			if (Scribe.mode != LoadSaveMode.LoadingVars) return;
			foreach (var data in _defReferencesByName.Values)
			{
				Logger.Gen($"Loading {data.Def.defName}: {data.Seed}");
				// Ensure that the TraderKindDef has been generated properly before loading any traders.
				GenWithSeed(data);
			}

		}
	}
}