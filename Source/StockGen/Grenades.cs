using System.Collections.Generic;
using System.Linq;
using Verse;

namespace TG.StockGen
{
	public class Grenades : ConditionMatcher
	{
		protected override bool CanBuy(in ThingDef def)
		{
			return DefOf.ThingCategory.Grenades.DescendantThingDefs.Contains(def) || def.apparel != null &&
				(def.apparel.tags.Contains("GrenadeDestructiveBelt") ||
				 def.apparel.tags.Contains("GrenadeNonDestructiveBelt"));
		}
	}
}