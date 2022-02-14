using Force.DeepCloner;
using System;
using System.Linq;
using RimWorld;
using Verse;

namespace TG.Gen
{
	/// <summary>
	/// Includes utilities for procedurally generating TraderKindDefs.
	/// </summary>
	public static class TraderKind
	{
		/// <summary>
		/// Maximum Markov chain depth allowed.
		/// </summary>
		private const int MaxDepth = 32;

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
		/// Procedurally generates a new TraderKindDef, and registers it into DefDatabase.
		/// </summary>
		/// <param name="genDef">Trader generation preset to use.</param>
		/// <param name="genSeed">Seed to use.</param>
		/// <returns>Newly generated TraderKindDef.</returns>
		public static TraderKindDef Gen(in TraderGenDef genDef, int genSeed)
		{
			var newDefName = genDef.defName + '_' + genSeed;

			// If the def already exists, assume that it has previously been created. This can happen while loading without
			// quitting RimWorld; in that case the def may still be there.
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


			Logger.Gen($"Generating new TraderKindDef {def.defName} using seed {genSeed}.");

			var rng = new Random(genSeed);

			foreach (var link in genDef.links)
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

			ShortHashGiver.GiveShortHash(def, def.GetType());
			DefDatabase<TraderKindDef>.Add(def);
			Logger.Gen($"Finished generating new TraderKindDef {def.defName}.");

			return def;
		}
	}
}