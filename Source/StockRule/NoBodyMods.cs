using Verse;

namespace TraderGen.StockRule
{
	/// <summary>
	/// Prevents trading and stocking body mods.
	/// </summary>
	public class NoBodyMods : Rule
	{
		public override bool ForbidsTrading(in ThingDef def)
		{
			return Things.Util.IsBodyMod(def);
		}

		public override bool ForbidsStocking(in ThingDef def)
		{
			return Things.Util.IsBodyMod(def);
		}
	}
}