using Verse;

namespace TG.StockGen
{
	public class Security : ConditionMatcher
	{
		protected override bool CanBuy(in ThingDef def)
		{
			return def.IsWithinCategory(DefOf.ThingCategory.BuildingsSecurity) && def.Minifiable &&
						 // Bear traps have null graphics unless they are actually built instead of being generated.
						 // See https://github.com/AndroidQuazar/VanillaFurnitureExpanded-Security/pull/5
			       def.defName != "VFES_BearTrap";
		}
	}
}