using RimWorld;
using Verse;

namespace TG.StockGen
{
	/// <summary>
	/// Matches ranged weapons.
	/// </summary>
	public class WeaponRanged : Weapon
	{
		protected override bool WeaponCheck(in ThingDef def) => def.IsRangedWeapon;

		/// <summary>
		/// Use the same weight calculation as StockGenerator_WeaponsRanged.
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