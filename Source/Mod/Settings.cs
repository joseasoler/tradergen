using System.Collections.Generic;
using RimWorld;
using TG.TraderKind;
using Verse;

namespace TG.Mod
{
	public class SettingValues
	{
		/// <summary>
		/// Determines if traders can have psylink neuroformers in stock.
		/// </summary>
		public bool SellPsylinkNeuroformers /* = false */;

		/// <summary>
		/// Period of orbital trader arrival in days.
		/// 0 means "Disable".
		/// </summary>
		public int PeriodOrbital /* = 0 */;

		/// <summary>
		/// Silver stock of each trader category in %.
		/// </summary>
		public Dictionary<TraderKindCategory, float> SilverScaling = new Dictionary<TraderKindCategory, float>
		{
			{TraderKindCategory.Orbital, 100.0f},
			{TraderKindCategory.Settlement, 100.0f},
			{TraderKindCategory.Caravan, 100.0f},
			{TraderKindCategory.Visitor, 100.0f}
		};

		/// <summary>
		/// Generate a detailed report of the trader generation process and append it to the log.
		/// </summary>
		public bool LogGen /* = false */;

		/// <summary>
		/// Add stock generator info to the trader generation process log. Only used if LogGen is true.
		/// </summary>
		public bool LogStockGen /* = false */;
	}

	/// <summary>
	/// Handles mod settings.
	/// </summary>
	public class Settings : ModSettings
	{
		/// <summary>
		/// Current values for all settings.
		/// </summary>
		private static SettingValues _values = new SettingValues();

		/// <summary>
		/// Period of orbital trader arrival in days.
		/// </summary>
		public static uint PeriodOrbital
		{
			get => (uint) _values.PeriodOrbital;
			set => _values.PeriodOrbital = (int) value;
		}

		/// <summary>
		/// Determines if traders can have psylink neuroformers in stock.
		/// Always returns false if Royalty is not active.
		/// </summary>
		public static bool SellPsylinkNeuroformers
		{
			get => _values.SellPsylinkNeuroformers && ModsConfig.RoyaltyActive;
			set => _values.SellPsylinkNeuroformers = value;
		}

		public static float GetSilverScaling(TraderKindCategory category)
		{
			return _values.SilverScaling[category];
		}

		public static void SetSilverScaling(TraderKindCategory category, float value)
		{
			_values.SilverScaling[category] = value;
		}

		/// <summary>
		/// Disables modification of the period of orbital traders.
		/// </summary>
		public const uint DisablePeriodOrbital = 0U;

		/// <summary>
		/// Maximum allowed value for PeriodOrbital settings in days.
		/// </summary>
		public const uint MaxPeriodOrbital = GenDate.DaysPerYear;

		/// <summary>
		/// Minimum allowed value for SilverScaling settings in %.
		/// </summary>
		public const float MinSilverScaling = 5.0f;

		/// <summary>
		/// Maximum allowed value for SilverScaling settings in %.
		/// </summary>
		public const float MaxSilverScaling = 1000.0f;

		/// <summary>
		/// Generate a detailed report of the trader generation process and append it to the log.
		/// </summary>
		public static bool LogGen
		{
			get => _values.LogGen;
			set => _values.LogGen = value;
		}

		/// <summary>
		/// Add stock generator info to the trader generation process log. Only used if LogGen is true.
		/// </summary>
		public static bool LogStockGen
		{
			get => _values.LogStockGen;
			set => _values.LogStockGen = value;
		}

		public static void Reset()
		{
			_values = new SettingValues();
		}

		/// <summary>
		/// Save and load preferences.
		/// </summary>
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref _values.SellPsylinkNeuroformers, "SellPsylinkNeuroformers");
			Scribe_Values.Look(ref _values.PeriodOrbital, "PeriodOrbital");
			Scribe_Collections.Look(ref _values.SilverScaling, "SilverScaling");
			Scribe_Values.Look(ref _values.LogGen, "LogGen");
			Scribe_Values.Look(ref _values.LogStockGen, "LogStockGen");
		}
	}
}