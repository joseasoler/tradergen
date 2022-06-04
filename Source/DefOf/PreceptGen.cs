using RimWorld;
using TG.Ideo;

namespace TG.DefOf
{
	[RimWorld.DefOf]
	public class PreceptGen
	{
		public static PreceptGenDef TG_AutomaticApprovesOfCharity;
		public static PreceptGenDef TG_AutomaticApprovesOfSlavery;
		public static PreceptGenDef TG_AutomaticDislikesHumanApparel;
		public static PreceptGenDef TG_AutomaticLikesHumanApparel;
		public static PreceptGenDef TG_AutomaticNoRawVegetables;

		static PreceptGen() => DefOfHelper.EnsureInitializedInCtor(typeof (PreceptGen));
	}
}