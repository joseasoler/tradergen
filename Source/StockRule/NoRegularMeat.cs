using TG.Things;
using Verse;

namespace TG.StockRule
{
	/// <summary>
	/// Prevents stocking any meat which is not insect or human meat (handled separately).
	/// Intended to be added automatically for precepts disallowing hunting camps.
	/// </summary>
	public class NoRegularMeat : Rule
	{
		public override bool ForbidsTrading(in ThingDef def)
		{
			return Util.IsRegularMeat(def);
		}
		
		public override bool ForbidsStocking(in ThingDef def)
		{
			return Util.IsRegularMeat(def);
		}
	}
}