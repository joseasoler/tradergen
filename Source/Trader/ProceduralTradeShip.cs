using System;
using System.Linq;
using Force.DeepCloner;
using RimWorld;
using Verse;

namespace TG.Trader
{
	/// <summary>
	/// Procedurally generated orbital trader ship.
	/// </summary>
	public class ProceduralTradeShip : TradeShip
	{
		/// <summary>
		/// Procedural generation template used to create the TraderKindDef.
		/// </summary>
		private TraderGenDef _genDef;

		/// <summary>
		/// Seed used to generate the TraderKindDef using the TraderGenDef.
		/// </summary>
		private int _genSeed;

		/// <summary>
		/// Obtain the Faction that the TradeShip will use.
		/// </summary>
		/// <param name="def">Procedural generation template.</param>
		/// <returns>Faction.</returns>
		private static Faction GetFaction(TraderGenDef def)
		{
			return Find.FactionManager.AllFactions.Where(faction => faction.def == def.faction)
				.TryRandomElement(out var result)
				? result
				: null;
		}

		private static void LogStockGenerator(in StockGenerator generator)
		{
			var text = generator.GetType().Name + ": ";
			if (generator.GetType() == typeof(StockGenerator_SingleDef))
			{
				var g = (StockGenerator_SingleDef) generator;
				text += g.thingDef.defName + " -> " + g.countRange;
			} else if (generator.GetType() == typeof(StockGenerator_BuySingleDef))
			{
				var g = (StockGenerator_BuySingleDef) generator;
				text += g.thingDef.defName + " -> " + g.countRange;
			}
			Logger.OrbitalTraderGen(text);
		}


		private static void AddLinkData(ref TraderKindDef def, in LinkDef linkDef, in int depth)
		{
			if (linkDef.stockGroups != null)
			{
				foreach (var stockGroup in linkDef.stockGroups)
				{
					Logger.OrbitalTraderGen($"{depth}: Processing StockGroup {stockGroup}.");

					def.stockGenerators.AddRange(stockGroup.generators);
					// StockGenerators have a reference to their TraderKindDef. This reference is not set properly for generators
					// coming from a StockGroupDef as these lack a TraderKindDef. Since multiple TraderKindDefs may be using the
					// same StockGroupDef at any given time, a shallow copy of the StockGenerator is provided to them instead.
					// Each copy can then point to the right TraderKindDef, and will be disposed off automatically when its
					// TraderKindDef is deleted.
					foreach (var stockGeneratorCopy in stockGroup.generators.Select(stockGenerator =>
						         stockGenerator.ShallowClone()))
					{
						def.stockGenerators.Add(stockGeneratorCopy);
						stockGeneratorCopy.ResolveReferences(def);
						Logger.OrbitalTraderGen($"{depth}: Adding StockGenerator {stockGeneratorCopy}.");
					}
				}
			}
		}

		private static void FollowLink(ref Random rng, ref TraderKindDef def, in LinkDef linkDef, int depth = 1)
		{
			Logger.OrbitalTraderGen($"{depth}: Arrived at link {linkDef.defName}.");
			if (depth >= 32)
			{
				Logger.Error("Maximum depth reached. No further links will be followed.");
				return;
			}

			AddLinkData(ref def, linkDef, depth);

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

			// It seems to be adding some links such as jello but later they do not show up in the stock. Great progress anyways!
			// They always have the same negotiator??

			FollowLink(ref rng, ref def, chosenLinkDef, depth + 1);
		}

		/// <summary>
		/// Procedurally generates a new TraderKindDef, and registers it into DefDatabase.
		/// </summary>
		/// <param name="genDef">Trader generation preset to use.</param>
		/// <param name="genSeed">Seed to use.</param>
		/// <returns>Newly generated TraderKindDef.</returns>
		private static TraderKindDef GenerateTraderKindDef(in TraderGenDef genDef, int genSeed)
		{
			var newDefName = genDef.defName + '_' + genSeed;

			// If the def already exists, assume that it has previously been created. This can happen while loading without
			// quitting RimWorld; in that case the def may still be there.
			DefDatabase<TraderKindDef>.defsByName.TryGetValue(newDefName, out var def);
			if (def != null)
			{
				Logger.OrbitalTraderGen($"TraderKindDef {newDefName} already exists, generation unnecessary.");
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


			Logger.OrbitalTraderGen($"Generating new TraderKindDef {def.defName} using seed {genSeed}.");

			var rng = new Random(genSeed);

			foreach (var link in genDef.links)
			{
				Logger.OrbitalTraderGen("Starting new chain.");
				FollowLink(ref rng, ref def, link);
			}

			Logger.OrbitalTraderGen("Final generators:");
			foreach (var generator in def.stockGenerators)
			{
				LogStockGenerator(generator);
			}

			def.PostLoad();

			ShortHashGiver.GiveShortHash(def, def.GetType());
			DefDatabase<TraderKindDef>.Add(def);
			Logger.OrbitalTraderGen($"Finished generating new TraderKindDef {def.defName}.");

			return def;
		}

		public ProceduralTradeShip()
		{
		}

		public ProceduralTradeShip(TraderGenDef traderGenDef, int seed) : base(GenerateTraderKindDef(traderGenDef, seed),
			GetFaction(traderGenDef))
		{
			_genDef = traderGenDef;
			_genSeed = seed;

			// ToDo procedural generation of names.
			/*
			this.name = NameGenerator.GenerateName(RulePackDefOf.NamerTraderGeneral, (IEnumerable<string>) TradeShip.tmpExtantNames);
			if (faction != null) {
				this.name = string.Format("{0} {1} {2}", (object) this.name, (object) "OfLower".Translate(), (object) faction.Name);
			}
			*/

			// Unused. Reduce space taken in saved games instead.
			randomPriceFactorSeed = 0;

			Logger.OrbitalTraderGen($"Generated new trade ship {name}");
		}

		public override void ExposeData()
		{
			Scribe_Defs.Look(ref _genDef, "genDef");
			Scribe_Values.Look(ref _genSeed, "genSeed");
			if (Scribe.mode == LoadSaveMode.LoadingVars)
			{
				// Ensure that the TraderKindDef has been generated properly before loading the base class.
				GenerateTraderKindDef(_genDef, _genSeed);
			}

			base.ExposeData();
		}

		/// <summary>
		/// After the orbital trader departs, its procedurally generated TraderKindDef must be removed from the game.
		/// </summary>
		public override void Depart()
		{
			base.Depart();

			var defName = def.defName;

			DefDatabase<TraderKindDef>.Remove(def);
			ShortHashGiver.takenHashesPerDeftype[def.GetType()].Remove(def.shortHash);
			Logger.OrbitalTraderGen($"Removed previously generated TraderKindDef {defName} and trade ship {name}");
		}
	}
}