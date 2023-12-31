using System;
using System.Collections.Generic;
using System.Text;
using RimWorld;
using TraderGen.StockModification;
using Verse;

namespace TraderGen.StockGen
{
	/// <summary>
	/// Generate items with a higher quality than normal.
	/// </summary>
	public abstract class HighQuality : ConditionMatcher
	{
		/// <summary>
		/// Minimum quality to generate, and also the most common value.
		/// </summary>
		private QualityCategory minQuality = QualityCategory.Awful;

		/// <summary>
		/// Maximum quality to generate, and also the most infrequent value.
		/// </summary>
		private QualityCategory maxQuality = QualityCategory.Awful;

		public override IEnumerable<string> ConfigErrors(TraderKindDef parentDef)
		{
			foreach (string error in base.ConfigErrors(parentDef))
			{
				yield return error;
			}

			if (minQuality > maxQuality)
			{
				yield return "TraderGen.StockGen.HighQuality: minQuality must be smaller than maxQuality.";
			}
		}

		public override void ConditionToText(ref StringBuilder b)
		{
			base.ConditionToText(ref b);
			b.Append($"minQuality: {Enum.GetName(typeof(QualityCategory), minQuality)}\n");
			b.Append($"maxQuality: {Enum.GetName(typeof(QualityCategory), maxQuality)}\n");
		}

		public override IEnumerable<Thing> GenerateThings(int forTile, Faction faction = null)
		{
			ChangeStockQuality.Set(minQuality, minQuality, maxQuality);
			foreach (Thing result in base.GenerateThings(forTile, faction))
			{
				yield return result;
			}

			ChangeStockQuality.Reset();
		}
	}
}