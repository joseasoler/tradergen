using System;
using System.Text;
using RimWorld;
using Verse;

namespace TG.StockGen
{
	/// <summary>
	/// Generates drugs. Alcoholic and non-alcoholic beverages are ignored as they have specific StockGens.
	/// </summary>
	public class Drugs : ConditionMatcher
	{
		/// <summary>
		/// If set to a specific value, it will only generate drugs from the chosen category.
		/// </summary>
		public DrugCategory drugCategory = DrugCategory.Any;

		public override void ConditionToText(ref StringBuilder b)
		{
			base.ConditionToText(ref b);
			b.Append($"drugCategory: {Enum.GetName(typeof(DrugCategory), drugCategory)}\n");
		}

		protected override bool CanBuy(in ThingDef def)
		{
			return Things.Util.IsDrug(def) && !Things.Util.IsAlcohol(def) &&
			       (drugCategory == DrugCategory.Any || def.ingestible.drugCategory == drugCategory);
		}
	}
}