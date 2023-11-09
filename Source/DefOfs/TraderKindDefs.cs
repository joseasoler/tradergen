using RimWorld;

namespace TraderGen.DefOfs
{
	[DefOf]
	public class TraderKindDefs
	{
		public static TraderKindDef TG_OrbitalDebug;

		static TraderKindDefs() => DefOfHelper.EnsureInitializedInCtor(typeof(TraderKindDefs));
	}
}