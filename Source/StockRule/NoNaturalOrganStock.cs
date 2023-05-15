using Verse;

namespace TraderGen.StockRule
{
	/// <summary>
	/// Traders will never have natural organs in stock.
	/// </summary>
	public class NoNaturalOrganStock : Rule
	{
		public override bool ForbidsStocking(in ThingDef def)
		{
			return def.IsNaturalOrgan;
		}
	}
}