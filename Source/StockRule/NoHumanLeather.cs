using Verse;

namespace TG.StockRule
{
	/// <summary>
	/// Prevents trading human leather.
	/// Intended to be added automatically for precepts who like human apparel.
	/// </summary>
	[RimWorld.DefOf]
	public class NoHumanLeather : Rule
	{
		public override bool ForbidsTrading(in ThingDef def)
		{
			return def == DefOf.Thing.Leather_Human;
		}

		public override bool ForbidsStocking(in ThingDef def)
		{
			return def == DefOf.Thing.Leather_Human;
		}
	}
}