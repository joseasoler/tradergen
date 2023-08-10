using RimWorld;
using Verse;

namespace TraderGen.DefOf
{
	[RimWorld.DefOf]
	public class Thing
	{
		[MayRequireIdeology] public static ThingDef GauranlenSeed;

		[MayRequireBiotech] public static ThingDef SignalChip;
		[MayRequireBiotech] public static ThingDef PowerfocusChip;
		[MayRequireBiotech] public static ThingDef NanostructuringChip;

		[MayRequire("sarg.alphagenes")] public static ThingDef AG_Alphapack;
		[MayRequire("sarg.alphagenes")] public static ThingDef AG_Mixedpack;

		static Thing() => DefOfHelper.EnsureInitializedInCtor(typeof(Thing));
	}
}