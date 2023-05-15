using RimWorld;
using Verse;

namespace TraderGen.StockGen
{
	/// <summary>
	/// Matches insect meat only.
	/// </summary>
	public class InsectMeat : ConditionMatcher
	{
		public InsectMeat()
		{
			thingDefCountRange = IntRange.one;
		}
		protected override bool CanBuy(in ThingDef def)
		{
			return def.IsIngestible && FoodUtility.GetMeatSourceCategory(def) == MeatSourceCategory.Insect;
		}
	}
}