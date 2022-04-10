using RimWorld;
using TG.Mod;
using Verse;

namespace TG.StockGen
{
	/// <summary>
	/// Adds psylink neuroformers to the trader stock if Royalty is enabled and the mod setting is enabled.
	/// </summary>
	public class PsylinkNeuroformer : ConditionMatcher
	{
		public PsylinkNeuroformer()
		{
			thingDefCountRange = IntRange.one;
		}

		protected override bool CanBuy(in ThingDef def)
		{
			return Settings.SellPsylinkNeuroformers && def == ThingDefOf.PsychicAmplifier;
		}
	}
}