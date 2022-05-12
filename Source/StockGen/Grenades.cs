using System.Linq;
using Verse;

namespace TG.StockGen
{
	/// <summary>
	/// Matches both vanilla grenades and grenade belts from Vanilla Weapons Expanded - Grenades.
	/// </summary>
	public class Grenades : ConditionMatcher
	{
		protected override bool CanBuy(in ThingDef def)
		{
			// Vanilla grenades belong to the Grenades ThingCategory.
			return DefOf.ThingCategory.Grenades.DescendantThingDefs.Contains(def) ||
			       // VWE-G grenade belts are apparel with one of these apparel tags.
			       def.apparel != null && (def.apparel.tags.Contains("GrenadeDestructiveBelt") ||
			                               def.apparel.tags.Contains("GrenadeNonDestructiveBelt"));
		}
	}
}