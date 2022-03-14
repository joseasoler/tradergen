using Force.DeepCloner;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using RimWorld.Planet;
using TG.Mod;
using TG.StockGen;
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

		/// <summary>
		/// Map tile considered as the origin of the trader.
		/// </summary>
		public BiomeDef BiomeDef;

		/// <summary>
		/// Faction of the trader.
		/// </summary>
		public FactionDef FactionDef;

		public void ExposeData()
		{
			Scribe_Defs.Look(ref Def, "Def");
			Scribe_Values.Look(ref Seed, "Seed");
			Scribe_Defs.Look(ref BiomeDef, "BiomeDef");
			Scribe_Defs.Look(ref FactionDef, "FactionDef");
		}
	}

	/// <summary>
	/// Procedurally generates TraderKindDefs and also manages their life cycle, save and load.
	/// </summary>
	public class TraderKind : WorldComponent
	{
		private Dictionary<string, TraderGenData> _defReferencesByName;

		public TraderKind(World world) : base(world)
		{
			_defReferencesByName = new Dictionary<string, TraderGenData>();
		}

		/// <summary>
		/// Adds stock group data found in a NodeDef to the TraderKindDef being generated.
		/// </summary>
		/// <param name="nodeDef">NodeDef being evaluated.</param>
		/// <param name="def">Trader being generated.</param>
		private static void ApplyGenerators(in NodeDef nodeDef, ref TraderKindDef def)
		{
			foreach (var generator in nodeDef.generators)
			{
				// StockGenerators have a reference to their TraderKindDef. This reference is not set for generators coming
				// from a StockGroupDef as these lack a TraderKindDef. Since multiple TraderKindDefs may be using the same
				// StockGroupDef at any given time, a shallow copy of the StockGenerator is provided to them instead.
				// Each copy can later point to the right TraderKindDef, and will be deleted when its TraderKindDef is deleted.
				var generatorCopy = generator.ShallowClone();
				def.stockGenerators.Add(generatorCopy);
				generatorCopy.ResolveReferences(def);
				var configErrors = generatorCopy.ConfigErrors(def).ToList();
				if (configErrors.Count > 0)
				{
					Log.Error($"{generatorCopy.GetType().Name} config error: {string.Join("\n", configErrors)}");
				}

				if (!Settings.LogGen || !Settings.LogStockGen) continue;
				Logger.Gen(Util.ToText(generatorCopy).ToString());
			}
		}

		/// <summary>
		/// Applies all changes required by a specific node to the generated trader.
		/// </summary>
		/// <param name="nodeDef">NodeDef being evaluated.</param>
		/// <param name="def">Trader definition being generated.</param>
		private static void ApplyNode(in NodeDef nodeDef, ref TraderKindDef def)
		{
			if (nodeDef.generators != null)
			{
				ApplyGenerators(nodeDef, ref def);
			}
		}

		/// <summary>
		/// Algorithm for procedural generation of traders.
		/// </summary>
		/// <param name="genDef">Pattern used to generate the trader.</param>
		/// <param name="biomeDef">Biome from which the trader is originating.</param>
		/// <param name="faction">Faction of the trader.</param>
		/// <param name="def">Trader definition being generated.</param>
		private static void ProcGen(in TraderGenDef genDef, in BiomeDef biomeDef, in Faction faction, ref TraderKindDef def)
		{
			const int maximumNodesThreshold = 128;

			var pending = new Queue<NodeDef>();
			pending.Enqueue(genDef.node);

			while (pending.Count > 0)
			{
				if (pending.Count > maximumNodesThreshold)
				{
					Logger.ErrorOnce(
						$"Error generating {genDef.defName}: maximum number of nodes allowed is {maximumNodesThreshold}");
					return;
				}

				var current = pending.Dequeue();
				ApplyNode(current, ref def);
				current.next?.Nodes(current, biomeDef, faction).ForEach(newNodeDef => pending.Enqueue(newNodeDef));
				Logger.Gen($"After processing {current.defName} there are {pending.Count} nodes in the pending queue.");
			}
		}

		/// <summary>
		/// Generates a new trader definition.
		/// </summary>
		/// <param name="genDef"></param>
		/// <param name="biomeDef"></param>
		/// <param name="faction"></param>
		/// <returns></returns>
		public TraderKindDef Generate(in TraderGenDef genDef, BiomeDef biomeDef = null, in Faction faction = null)
		{
			if (Rand.stateStack.Count == 0)
			{
				Logger.ErrorOnce("TraderKindDef generation started without a prior Rand.PushState call.");
			}

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

			ProcGen(genDef, biomeDef, faction, ref def);

			def.PostLoad();

			// _defReferencesByName will already have a newDefName entry when called from ExposeData.
			_defReferencesByName.TryAdd(newDefName, new TraderGenData
			{
				Def = genDef,
				Seed = seed,
				BiomeDef = biomeDef,
				FactionDef = faction?.def
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
				Logger.Gen($"Loading {data.Def.defName}: {data.Seed}, {data.BiomeDef}, {data.FactionDef}");
				Rand.PushState(data.Seed);
				Generate(data.Def, data.BiomeDef, Find.FactionManager.FirstFactionOfDef(data.FactionDef));
				Rand.PopState();
			}
		}
	}
}