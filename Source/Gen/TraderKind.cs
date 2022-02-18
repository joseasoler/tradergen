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
	internal class TraderGenData : IExposable
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

	internal abstract class LinkProcessor
	{
		public abstract void Process(in LinkDef linkDef, in int depth);
	}

	internal class TraderKindDefProcessor : LinkProcessor
	{
		private TraderKindDef _def;

		public TraderKindDef Def => _def;

		public TraderKindDefProcessor(TraderKindDef def)
		{
			_def = def;
		}

		public override void Process(in LinkDef linkDef, in int depth)
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
						_def.stockGenerators.Add(stockGeneratorCopy);
						stockGeneratorCopy.ResolveReferences(_def);
						Logger.Gen(
							$"{depth}: Adding StockGenerator {Logger.StockGen(stockGeneratorCopy)}");
					}
				}
			}
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
		/// Visits a Markov chain link, processes it, and chooses the next link to explore (if any).
		/// </summary>
		/// <param name="proc">Processor that will be called for each visited link.</param>
		/// <param name="linkDef">LinkDef in which the chain is currently on.</param>
		/// <param name="depth">Depth of the current Markov chain.</param>
		private static void VisitLink(LinkProcessor proc, in LinkDef linkDef, int depth = 1)
		{
			Logger.Gen($"{depth}: Arrived at link {linkDef.defName}.");
			if (depth >= MaxDepth)
			{
				Logger.Error($"Maximum depth {MaxDepth} reached. No further links will be followed.");
				return;
			}

			proc.Process(linkDef, depth);

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
				var chosenCommonality = Rand.RangeInclusive(0, totalCommonality);
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

			VisitLink(proc, chosenLinkDef, depth + 1);
		}

		/// <summary>
		/// Procedurally generates a new TraderKindDef if necessary, and registers it into DefDatabase.
		/// A new seed must have been pushed into Rand prior to calling this method.
		/// </summary>
		/// <param name="genDef">Trader generation preset.</param>
		/// <param name="proc">Processor that will be called for each visited link.</param>
		private static void VisitAll(in TraderGenDef genDef, LinkProcessor proc)
		{
			if (Rand.stateStack.Count == 0)
			{
				Logger.ErrorOnce("TraderKindDef generation started without a prior Rand.PushState call.");
			}

			foreach (var link in genDef.links)
			{
				Logger.Gen("Starting new chain.");
				VisitLink(proc, link);
			}
		}

		public TraderKindDef Generate(in TraderGenDef genDef)
		{
			var seed = (int) Rand.seed;
			var newDefName = genDef.defName + '_' + seed;

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
				label = genDef.label,
				modExtensions = genDef.modExtensions,
				modContentPack = genDef.modContentPack,
				fileName = genDef.fileName,
				generated = true,
				orbital = genDef.orbital,
				requestable = genDef.requestable,
				hideThingsNotWillingToTrade = genDef.hideThingsNotWillingToTrade,
				tradeCurrency = genDef.tradeCurrency,
				faction = genDef.faction,
				permitRequiredForTrading = genDef.permitRequiredForTrading
			};

			Logger.Gen($"Generating new TraderKindDef {def.defName}.");
			
			VisitAll(genDef, new TraderKindDefProcessor(def));

			Logger.Gen("Final generators:");
			foreach (var generator in def.stockGenerators)
			{
				Logger.Gen($"\t{Logger.StockGen(generator)}");
			}

			def.PostLoad();

			// _defReferencesByName will already have a newDefName entry when called from ExposeData.
			_defReferencesByName.TryAdd(newDefName, new TraderGenData
			{
				Def = genDef,
				Seed = seed
			});
			ShortHashGiver.GiveShortHash(def, def.GetType());
			DefDatabase<TraderKindDef>.Add(def);
			Logger.Gen($"Finished generating new TraderKindDef {def.defName}.");

			return def;
		}

		/// <summary>
		/// Removes a procedurally generated TraderKindDef from the game.
		/// </summary>
		/// <param name="def">TraderKindDef that must be removed.</param>
		public void Remove(in TraderKindDef def)
		{
			var defName = def.defName;
			if (!def.generated)
			{
				Logger.ErrorOnce($"Attempted removal of non-generated TraderKindDef {defName}");
				return;
			}

			_defReferencesByName.Remove(def.defName);
			DefDatabase<TraderKindDef>.Remove(def);
			ShortHashGiver.takenHashesPerDeftype[def.GetType()].Remove(def.shortHash);
			Logger.Gen($"Removed previously generated TraderKindDef {defName}.");
		}

		/// <summary>
		/// Handles saving and loading of currently generated defs. Generates them on load as needed.
		/// </summary>
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Collections.Look(ref _defReferencesByName, "_defReferencesByName", LookMode.Value, LookMode.Deep);
			if (Scribe.mode != LoadSaveMode.LoadingVars) return;
			foreach (var data in _defReferencesByName.Values)
			{
				Logger.Gen($"Loading {data.Def.defName}: {data.Seed}");
				// Ensure that the TraderKindDef has been generated properly before loading any traders.
				Rand.PushState(data.Seed);
				Generate(data.Def);
				Rand.PopState();
			}
		}
	}
}