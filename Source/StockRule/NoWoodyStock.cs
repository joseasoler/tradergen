using TG.Things;
using Verse;

namespace TG.StockRule
{
	/// <summary>
	/// Prevents stocking any woody materials.
	/// Intended to be added automatically for precepts disallowing logging camps.
	/// </summary>
	public class NoWoodyStock : Rule
	{
		public override bool ForbidsStocking(in ThingDef def)
		{
			return Util.IsWoodyStuff(def);
		}
	}
}