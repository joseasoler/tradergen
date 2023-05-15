using System.Collections.Generic;
using System.Text;
using RimWorld;
using Verse;

namespace TraderGen.StockGen
{
	public class HighQualityWeapons : HighQuality
	{
		public List<string> weaponTags = new List<string>();
		
		public override IEnumerable<string> ConfigErrors(TraderKindDef parentDef)
		{
			foreach (var err in base.ConfigErrors(parentDef))
			{
				yield return err;
			}

			if (weaponTags.Count == 0)
			{
				yield return "TraderGen.StockGen.HighQualityWeapons: empty weaponTags list";
			}
		}
		
		public override void ConditionToText(ref StringBuilder b)
		{
			base.ConditionToText(ref b);
			Util.ToText(ref b, "weaponTags", weaponTags);
		}
		
		protected override bool CanBuy(in ThingDef def)
		{
			return def.IsWeapon && def.weaponTags != null && def.weaponTags.Any(weaponTag => weaponTags.Contains(weaponTag));
		}

	}
}