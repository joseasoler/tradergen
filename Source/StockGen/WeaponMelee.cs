using RimWorld;
using Verse;

namespace TraderGen.StockGen
{
	/// <summary>
	/// Matches melee weapons.
	/// </summary>
	public class WeaponMelee : Weapon
	{
		protected override bool WeaponCheck(in ThingDef def) => def.IsMeleeWeapon;

		/// <summary>
		/// Use the same weight calculation as StockGenerator_WeaponsMelee.
		/// </summary>
		/// <param name="def">Thing being checked</param>
		/// <param name="forTile">Tile in which the transaction takes place.</param>
		/// <param name="faction">Faction of the trader.</param>
		/// <returns>Random weight of the def.</returns>
		protected override float Weight(in ThingDef def, in int forTile, in Faction faction)
		{
			return StockGenerator_MarketValue.SelectionWeightMarketValueCurve.Evaluate(def.BaseMarketValue);
		}
	}
}