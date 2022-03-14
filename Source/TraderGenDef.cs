using RimWorld;
using Verse;

namespace TG
{
	/// <summary>
	/// Used to generate TraderKindDefs procedurally. Undocumented attributes are identical to those in TraderKindDef. 
	/// </summary>
	public class TraderGenDef : Def
	{
		public bool orbital;

		public bool requestable = true;

		public bool hideThingsNotWillingToTrade;

		public float commonality = 1f;

		public TradeCurrency tradeCurrency;

		public FactionDef faction;

		public RoyalTitlePermitDef permitRequiredForTrading;

		/// <summary>
		/// Starting point of the procedural generation.
		/// </summary>
		public NodeDef node;
	}
}