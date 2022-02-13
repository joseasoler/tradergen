using System.Linq;
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

		public ProceduralTradeShip()
		{
		}

		public ProceduralTradeShip(TraderGenDef traderGenDef, int seed) : base(Gen.TraderKind.Gen(traderGenDef, seed),
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

			// ToDo check other existing attributes to see if they are also unused.
			// Unused. Reduce space taken in saved games instead.
			randomPriceFactorSeed = 0;

			Logger.Gen($"Generated new trade ship {name}");
		}

		public override void ExposeData()
		{
			Scribe_Defs.Look(ref _genDef, "genDef");
			Scribe_Values.Look(ref _genSeed, "genSeed");
			if (Scribe.mode == LoadSaveMode.LoadingVars)
			{
				// Ensure that the TraderKindDef has been generated properly before loading the base class.
				Gen.TraderKind.Gen(_genDef, _genSeed);
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
			Logger.Gen($"Removed previously generated TraderKindDef {defName} and trade ship {name}");
		}
	}
}