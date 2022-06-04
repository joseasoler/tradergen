using RimWorld;
using Verse;

namespace TG.DefOf
{
	[RimWorld.DefOf]
	public class Thing
	{
		public static ThingDef Leather_Human;

		static Thing() => DefOfHelper.EnsureInitializedInCtor(typeof (Thing));
	}
}