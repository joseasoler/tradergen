using RimWorld.Planet;
using Verse;

namespace TraderGen.Harmony.Settlements
{
	public static class SettlementPatchUtil
	{
		public static void RegenerateStockIfRequired(Settlement settlement)
		{
			var trader = settlement.trader;
			if (trader.stock == null)
			{
				trader.RegenerateStock();
			}
		}
	}
}