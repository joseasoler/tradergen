using Verse;

namespace TraderGen.StockRule
{
	/// <summary>
	/// Traders will never purchase or stock natural organs.
	/// </summary>
	public class NoNaturalOrgans : NoNaturalOrganStock
	{
		public override bool ForbidsTrading(in ThingDef def)
		{
			return ForbidsStocking(def);
		}
	}
}