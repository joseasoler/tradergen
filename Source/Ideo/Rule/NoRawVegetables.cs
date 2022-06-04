using TG.Things;
using Verse;

namespace TG.Ideo.Rule
{
	/// <summary>
	/// Prevents stocking any raw vegetable or fruit.
	/// Intended to be added automatically for precepts disallowing farming camps.
	/// </summary>
	public class NoRawVegetables : Rule
	{
		public override bool ForbidsPurchase(in ThingDef def)
		{
			return Util.IsRawVegetable(def);
		}

		public override bool ForbidsStocking(in ThingDef def)
		{
			return Util.IsRawVegetable(def);
		}
	}
}