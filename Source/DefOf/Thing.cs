using RimWorld;
using Verse;

namespace TG.DefOf
{
	[RimWorld.DefOf]
	public class Thing
	{
		public static ThingDef Leather_Human;
		[MayRequireIdeology] public static ThingDef GauranlenSeed;

		static Thing() => DefOfHelper.EnsureInitializedInCtor(typeof(Thing));
	}
}