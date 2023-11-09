using RimWorld;
using TraderGen.DefOfs;
using Verse;

namespace TraderGen.StockGen
{
	/// <summary>
	/// Any food having the Gluttonous or VCE_Confectionery JoyKinds.
	/// </summary>
	public class FoodGluttonous : ConditionMatcher
	{
		protected override bool CanBuy(in ThingDef def)
		{
			return def.IsIngestible && def.ingestible.joyKind != null &&
				(def.ingestible.joyKind == JoyKindDefOf.Gluttonous || def.ingestible.joyKind == JoyKind.VCE_Confectionery);
		}
	}
}