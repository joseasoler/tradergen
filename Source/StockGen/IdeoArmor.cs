using Verse;

namespace TraderGen.StockGen
{
	/// <summary>
	/// Purchases and sells armor. When selling them, they have the color of the trader's ideology.
	/// </summary>
	public class IdeoArmor : IdeoColorApparel
	{
		protected override bool ApparelCondition(in ThingDef def)
		{
			return Things.Util.IsArmor(def);
		}
	}
}