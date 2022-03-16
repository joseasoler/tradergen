using RimWorld;
using Verse;

namespace TG.StockGen
{
	/// <summary>
	/// Any food having the Gluttonous or VCE_Confectionery JoyKinds.
	/// </summary>
	public class FoodGluttonous : ConditionMatcher
	{
		protected override bool CanBuy(in ThingDef def)
		{
			return def.IsIngestible && def.ingestible.joyKind != null && (def.ingestible.joyKind == JoyKindDefOf.Gluttonous ||
			                                                              def.ingestible.joyKind.defName ==
			                                                              "VCE_Confectionery");
		}
	}
}