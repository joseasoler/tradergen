using Verse;

namespace TraderGen.StockRule
{
	public class NoLeather : Rule
	{
		public override bool ForbidsTrading(in ThingDef def)
		{
			return def.IsStuff && def.IsLeather;
		}

		public override bool ForbidsStocking(in ThingDef def)
		{
			return def.IsStuff && def.IsLeather;
		}
	}
}