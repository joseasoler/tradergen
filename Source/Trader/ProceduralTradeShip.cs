using System.Linq;
using RimWorld;
using TG.Gen;
using Verse;

namespace TG.Trader
{
	/// <summary>
	/// Procedurally generated orbital trader ship.
	/// </summary>
	public class ProceduralTradeShip : TradeShip
	{
		/// <summary>
		/// Obtain the Faction that the TradeShip will use.
		/// </summary>
		/// <param name="def">Procedural generation template.</param>
		/// <returns>Faction.</returns>
		private static Faction GetFaction(TraderKindDef def)
		{
			return Find.FactionManager.AllFactions.Where(faction => faction.def == def.faction)
				.TryRandomElement(out var result)
				? result
				: null;
		}

		public ProceduralTradeShip()
		{
		}

		public ProceduralTradeShip(TraderKindDef def) : base(def, GetFaction(def))
		{
			// Unused. Reduce space taken in saved games instead.
			randomPriceFactorSeed = 0;
		}

		/// <summary>
		/// After the orbital trader departs, its procedurally generated TraderKindDef must be removed from the game.
		/// </summary>
		public override void Depart()
		{
			base.Depart();
			Find.World.GetComponent<TraderKind>().Remove(def);
		}
	}
}