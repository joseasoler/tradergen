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

		[MayRequireBiotech] [MayRequire("sarg.alphamechs")]
		public static ThingDef AM_HyperLinkageChip;
		[MayRequireBiotech] [MayRequire("sarg.alphamechs")]
		public static ThingDef AM_StellarProcessingChip;
		[MayRequireBiotech] [MayRequire("sarg.alphamechs")]
		public static ThingDef AM_QuantumMatrixChip;

		static Thing() => DefOfHelper.EnsureInitializedInCtor(typeof(Thing));
	}
}