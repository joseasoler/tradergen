using RimWorld;

namespace TG.DefOf
{
	[RimWorld.DefOf]
	public class JoyKind
	{
		public static JoyKindDef Chemical;

		static JoyKind() => DefOfHelper.EnsureInitializedInCtor(typeof (JoyKind));
	}
}