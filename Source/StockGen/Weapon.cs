using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace TG.StockGen
{
	/// <summary>
	/// Matches weapons which follow a certain condition (specified by child classes) and match certain weapon tags.
	/// </summary>
	public abstract class Weapon : ConditionMatcher
	{
		/// <summary>
		/// The weapon must have one of these weapon tags.
		/// </summary>
		public List<string> weaponTags;

		/// <summary>
		/// The weapon must not have any of these weapon tags.
		/// </summary>
		public List<string> excludeWeaponTags;

		/// <summary>
		/// Condition which must be implemented by child classes.
		/// </summary>
		/// <param name="def">Thing being checked.</param>
		/// <returns>True if this StockGenerator should process this weapon.</returns>
		protected abstract bool WeaponCheck(in ThingDef def);

		protected override bool CanBuy(in ThingDef def)
		{
			return def.IsWeapon && WeaponCheck(def) &&
			       (weaponTags == null || def.weaponTags != null && def.weaponTags.Intersect(weaponTags).Any()) &&
			       (excludeWeaponTags == null ||
			        def.weaponTags != null && !def.weaponTags.Intersect(excludeWeaponTags).Any());
		}
	}
}