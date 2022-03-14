using System.Collections.Generic;
using Verse;

namespace TG.Mod
{
	public class SettingValues
	{
		/// <summary>
		/// Maximum number of orbital ships allowed at the same time.
		/// </summary>
		public int MaxOrbitalShips = 5;

		/// <summary>
		/// Silver stock of orbital traders in %.
		/// </summary>
		public float OrbitalSilverScaling = 100.0f;

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
		private static SettingValues _values = new SettingValues();

		/// <summary>
		/// Maximum number of orbital ships allowed at the same time.
		/// </summary>
		public static int MaxOrbitalShips
		{
			get => _values.MaxOrbitalShips;
			set => _values.MaxOrbitalShips = value;
		}

		/// <summary>
		/// Silver stock of orbital traders in %.
		/// </summary>
		public static float OrbitalSilverScaling
		{
			get => _values.OrbitalSilverScaling;
			set => _values.OrbitalSilverScaling = value;
		}

		/// <summary>
		/// Minimum allowed value for OrbitalSilverScaling.
		/// </summary>
		public const float MinSilverScaling = 5.0f;

		/// <summary>
		/// Maximum allowed value for OrbitalSilverScaling.
		/// </summary>
		public const float MaxSilverScaling = 500.0f;

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
			Scribe_Values.Look(ref _values.MaxOrbitalShips, "MaxOrbitalShips");
			Scribe_Values.Look(ref _values.OrbitalSilverScaling, "OrbitalSilverScaling");
			Scribe_Values.Look(ref _values.LogGen, "LogGen");
			Scribe_Values.Look(ref _values.LogStockGen, "LogStockGen");
		}
	}
}