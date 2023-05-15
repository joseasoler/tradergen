using System.Collections.Generic;
using RimWorld;
using Verse;

namespace TraderGen.StockRule
{
	/// <summary>
	/// Forbids all drugs from the specified categories.
	/// </summary>
	public class NoDrugs : Rule
	{
		/// <summary>
		/// Forbidden drug categories.
		/// </summary>
		public List<DrugCategory> drugCategories = new List<DrugCategory>();

		private bool ShouldForbid(in ThingDef def)
		{
			return Things.Util.IsDrug(def) && drugCategories.Contains(def.ingestible.drugCategory);
		}

		public override bool ForbidsTrading(in ThingDef def)
		{
			return ShouldForbid(def);
		}

		public override bool ForbidsStocking(in ThingDef def)
		{
			return ShouldForbid(def);
		}
	}
}