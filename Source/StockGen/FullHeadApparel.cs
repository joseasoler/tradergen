using RimWorld;
using Verse;

namespace TraderGen.StockGen
{
	/// <summary>
	/// Generates non-armor apparel that fully covers the head of the pawn.
	/// </summary>
	public class FullHeadApparel : IdeoColorApparel
	{
		protected override bool ApparelCondition(in ThingDef def)
		{
			return def.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.FullHead) && !Things.Util.IsArmor(def);
		}
	}
}