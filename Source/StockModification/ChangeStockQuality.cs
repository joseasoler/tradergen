using RimWorld;

namespace TraderGen.StockModification
{
	/// <summary>
	/// Temporarily changes the quality levels of items generated for trader stock.
	/// See CompQuality_PostPostGeneratedForTrader_Patch for details.
	/// </summary>
	public static class ChangeStockQuality
	{
		public class QualityRange
		{
			public readonly QualityCategory MinQuality;
			public readonly QualityCategory CenterQuality;
			public readonly QualityCategory MaxQuality;

			public QualityRange(QualityCategory min, QualityCategory center, QualityCategory max)
			{
				MinQuality = min;
				CenterQuality = center;
				MaxQuality = max;
			}
		}

		private static QualityRange _qualityRange;

		public static void Set(QualityCategory min, QualityCategory center, QualityCategory max)
		{
			_qualityRange = new QualityRange(min, center, max);
		}

		public static void Reset()
		{
			_qualityRange = null;
		}

		public static QualityRange Get()
		{
			return _qualityRange;
		}
	}
}