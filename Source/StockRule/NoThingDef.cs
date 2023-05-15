using Verse;

namespace TraderGen.StockRule
{
	/// <summary>
	/// Prevents trading and stocking a specific ThingDef.
	/// </summary>
	[RimWorld.DefOf]
	public class NoThingDef : Rule
	{
		/// <summary>
		/// This def will never be traded or stocked by this trader.
		/// </summary>
		public ThingDef thingDef;

		public override bool ForbidsTrading(in ThingDef def)
		{
			return def == thingDef;
		}

		public override bool ForbidsStocking(in ThingDef def)
		{
			return def == thingDef;
		}
	}
}