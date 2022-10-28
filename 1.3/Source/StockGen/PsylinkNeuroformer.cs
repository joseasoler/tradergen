using RimWorld;
using TG.Mod;
using Verse;

namespace TG.StockGen
{
	/// <summary>
	/// Adds psylink neuroformers to the trader stock if Royalty is enabled and the mod setting is enabled.
	/// The Empire will never sell psylink neuroformers.
	/// </summary>
	public class PsylinkNeuroformer : ConditionMatcher
	{
		private Faction _faction;

		public PsylinkNeuroformer()
		{
			thingDefCountRange = IntRange.one;
		}

		public override void BeforeGen(in int tile, in Faction faction)
		{
			base.BeforeGen(tile, faction);
			_faction = faction;
		}

		protected override bool CanSell(in ThingDef def, in int forTile, in Faction faction)
		{
			return base.CanSell(def, forTile, faction) && (_faction == null || _faction.def != FactionDefOf.Empire);
		}

		protected override bool CanBuy(in ThingDef def)
		{
			return Settings.SellPsylinkNeuroformers && def == ThingDefOf.PsychicAmplifier;
		}
	}
}