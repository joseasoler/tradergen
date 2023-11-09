using RimWorld;
using Verse;

namespace TraderGen.DefOfs
{
	[DefOf]
	public static class GeneDefOfs
	{
		[MayRequire("RedMattis.Undead")]
		public static GeneDef VU_WhiteRoseBite;

		static GeneDefOfs() => DefOfHelper.EnsureInitializedInCtor(typeof(GeneDefOfs));
	}
}