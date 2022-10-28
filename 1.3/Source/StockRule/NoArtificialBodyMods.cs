using Verse;

namespace TG.StockRule
{
	/// <summary>
	/// Prevents trading and stocking artificial body mods.
	/// </summary>
	public class NoArtificialBodyMods : Rule
	{
		public override bool ForbidsTrading(in ThingDef def)
		{
			return Things.Util.IsArtificialBodyMod(def);
		}

		public override bool ForbidsStocking(in ThingDef def)
		{
			return Things.Util.IsArtificialBodyMod(def);
		}
	}
}