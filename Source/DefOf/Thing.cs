using RimWorld;
using Verse;

namespace TG.DefOf
{
	[RimWorld.DefOf]
	public class Thing
	{
		[MayRequireIdeology] public static ThingDef GauranlenSeed;

		static Thing() => DefOfHelper.EnsureInitializedInCtor(typeof(Thing));
	}
}