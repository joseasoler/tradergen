using System.Collections.Generic;
using Verse;

namespace TG
{
	/// <summary>
	/// Provides extra data for trader generation.
	/// StockGenerators chosen from here will be added to the ones in the TraderKindDef.
	/// </summary>
	public class GenExtension : DefModExtension
	{
		/// <summary>
		/// List of specializations allowed for this trader type.
		/// </summary>
		public List<SpecializationCommonality> specializations = new List<SpecializationCommonality>();

		/// <summary>
		/// Extra name rules used for orbital traders.
		/// </summary>
		public RulePackDef extraNameRules;
	}
}