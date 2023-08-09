using RimWorld;
using Verse;

namespace TraderGen.DefOf
{
	[RimWorld.DefOf]
	public class Thing
	{
		[MayRequireIdeology] public static ThingDef GauranlenSeed;
		[MayRequireBiotech] public static ThingDef SubcoreBasic;
		[MayRequireBiotech] public static ThingDef SubcoreRegular;
		[MayRequireBiotech] public static ThingDef SubcoreHigh;

		[MayRequire("sarg.alphagenes")] public static ThingDef AG_Alphapack;

		[MayRequire("sarg.alphagenes")] public static ThingDef AG_Mixedpack;
		static Thing() => DefOfHelper.EnsureInitializedInCtor(typeof(Thing));
	}
}