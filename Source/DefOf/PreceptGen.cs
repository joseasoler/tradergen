using RimWorld;
using TraderGen.Ideo;

namespace TraderGen.DefOf
{
	[RimWorld.DefOf]
	public class PreceptGen
	{
		[MayRequireIdeology] public static PreceptGenDef TG_AutomaticApprovesOfCharity;
		[MayRequireIdeology] public static PreceptGenDef TG_AutomaticApprovesOfSlavery;
		[MayRequireIdeology] public static PreceptGenDef TG_AutomaticDislikesHumanApparel;
		[MayRequireIdeology] public static PreceptGenDef TG_AutomaticLikesHumanApparel;
		[MayRequireIdeology] public static PreceptGenDef TG_AutomaticNoRawVegan;
		[MayRequireIdeology] public static PreceptGenDef TG_AutomaticNoRegularMeat;
		[MayRequireIdeology] public static PreceptGenDef TG_AutomaticNoWoodyStock;
		[MayRequireIdeology] public static PreceptGenDef TG_AutomaticPreferredApparel;
		[MayRequireIdeology] public static PreceptGenDef TG_AutomaticVeneratedAnimal;

		static PreceptGen() => DefOfHelper.EnsureInitializedInCtor(typeof(PreceptGen));
	}
}