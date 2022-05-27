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
		public bool SellPsylinkNeuroformers = true;

		/// <summary>
		/// Orbital traders will have a random number of specializations in this interval.
		/// Set both values to zero to disable specializations.
		/// </summary>
		public IntRange OrbitalSpecializations = IntRange.one;

		/// <summary>
		/// Disable TraderKindDef.commonalityMultFromPopulationIntent calculations.
		/// </summary>
		public bool IgnoreColonyPopulationCommonality /* = false */;

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
		/// Determines if traders can have psylink neuroformers in stock.
		/// Always returns false if Royalty is not active.
		/// </summary>
		public static bool SellPsylinkNeuroformers
		{
			get => _values.SellPsylinkNeuroformers && ModsConfig.RoyaltyActive;
			set => _values.SellPsylinkNeuroformers = value;
		}

		public static IntRange OrbitalSpecializations
		{
			get => _values.OrbitalSpecializations;
			set => _values.OrbitalSpecializations = value;
		}
		
		
		/// <summary>
		/// Ignore colony population when calculating trader commonality.
		/// </summary>
		public static bool IgnoreColonyPopulationCommonality
		{
			get => _values.IgnoreColonyPopulationCommonality;
			set => _values.IgnoreColonyPopulationCommonality = value;
		}

		/// <summary>
		/// Minimum allowed value for SilverScaling settings in %.
		/// </summary>
		public const float MinSilverScaling = 5.0f;

		/// <summary>
		/// Maximum allowed value for SilverScaling settings in %.
		/// </summary>
		public const float MaxSilverScaling = 1000.0f;

		/// <summary>
		/// Maximum value allowed for OrbitalSpecializations.
		/// </summary>
		public const int MaxOrbitalSpecializations = 5;

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

			var orbitalSpecializationsMin = _values.OrbitalSpecializations.min;
			var orbitalSpecializationsMax = _values.OrbitalSpecializations.max;
			Scribe_Values.Look(ref orbitalSpecializationsMin, "OrbitalSpecializationsMin", 1);
			Scribe_Values.Look(ref orbitalSpecializationsMax, "OrbitalSpecializationsMax", 1);
			_values.OrbitalSpecializations = new IntRange(orbitalSpecializationsMin, orbitalSpecializationsMax);

			Scribe_Values.Look(ref _values.IgnoreColonyPopulationCommonality, "IgnoreColonyPopulationCommonality");

			Scribe_Values.Look(ref _values.LogGen, "LogGen");
			Scribe_Values.Look(ref _values.LogStockGen, "LogStockGen");
		}
	}
}