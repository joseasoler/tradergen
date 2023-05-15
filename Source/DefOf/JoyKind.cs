using RimWorld;

namespace TraderGen.DefOf
{
	[RimWorld.DefOf]
	public class JoyKind
	{
		public static JoyKindDef Chemical;

		static JoyKind() => DefOfHelper.EnsureInitializedInCtor(typeof (JoyKind));
	}
}