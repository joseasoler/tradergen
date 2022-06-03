using Verse;

namespace TG.Ideo.Rule
{
	/// <summary>
	/// Prevents purchasing human leather.
	/// Intended to be added automatically for precepts who like human apparel.
	/// </summary>
	[RimWorld.DefOf]
	public class NoHumanLeatherPurchase : Rule
	{
		public override bool ForbidsPurchase(in ThingDef def)
		{
			return def == DefOf.Thing.Leather_Human;
		}
	}
}