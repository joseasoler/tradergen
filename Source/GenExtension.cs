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
		public List<SpecializationCommonality> specializations = new List<SpecializationCommonality>();
	}
}