using RimWorld;

namespace TraderGen.DefOfs
{
	[DefOf]
	public class JoyKind
	{
		public static JoyKindDef Chemical;
		
		[MayRequire("VanillaExpanded.VCookE")]
		public static JoyKindDef VCE_Confectionery;

		static JoyKind() => DefOfHelper.EnsureInitializedInCtor(typeof (JoyKind));
	}
}