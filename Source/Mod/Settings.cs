using System.Collections.Generic;
using RimWorld;
using TraderGen.TraderKind;
using Verse;

// ToDo change namespace without affecting settings.
namespace TG.Mod
{
	public class SettingValues
	{
		/// <summary>
		/// Determines if traders can have psylink neuroformers in stock.
		/// </summary>
		public bool SellPsylinkNeuroformers = true;

		/// <summary>
		/// Caravans will have a random number of specializations in this interval.
		/// Set both values to zero to disable specializations.
		/// </summary>
		public IntRange CaravanSpecializations = IntRange.one;

		/// <summary>
		/// Orbital traders will have a random number of specializations in this interval.
		/// Set both values to zero to disable specializations.
		/// </summary>
		public IntRange OrbitalSpecializations = IntRange.one;

		/// <summary>
		/// Some ideologies will add extra stock to the trader that follows them. For example, ideologies approving
		/// charity may have some medicine in stock while ideologies that love insect meat will always purchase it.
		/// </summary>
		public bool IdeologyMayAddStock = true;

		/// <summary>
		/// When this setting is enabled, certain ideologies will refuse to stock or trade certain items. For example most
		/// ideologies will reject purchasing human leather and human meat. Body purists will reject purchasing bionics.
		/// </summary>
		public bool IdeologyMayForbidTrading = true;

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

		public static IntRange CaravanSpecializations
		{
			get => _values.CaravanSpecializations;
			set => _values.CaravanSpecializations = value;
		}

		public static IntRange OrbitalSpecializations
		{
			get => _values.OrbitalSpecializations;
			set => _values.OrbitalSpecializations = value;
		}

		/// <summary>
		/// Some ideologies will add extra stock to the trader that follows them.
		/// </summary>
		public static bool IdeologyMayAddStock
		{
			get => ModsConfig.IdeologyActive && _values.IdeologyMayAddStock;
			set => _values.IdeologyMayAddStock = value;
		}

		/// <summary>
		/// When this setting is enabled, certain ideologies will refuse to stock or trade certain items.
		/// </summary>
		public static bool IdeologyMayForbidTrading
		{
			get => ModsConfig.IdeologyActive && _values.IdeologyMayForbidTrading;
			set => _values.IdeologyMayForbidTrading = value;
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
		/// Maximum value allowed for Specializations.
		/// </summary>
		public const int MaxSpecializations = 5;

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
			if (ModsConfig.RoyaltyActive)
			{
				Scribe_Values.Look(ref _values.SellPsylinkNeuroformers, "SellPsylinkNeuroformers", true);
			}
			
			var caravanSpecializationsMin = _values.CaravanSpecializations.min;
			var caravanSpecializationsMax = _values.CaravanSpecializations.max;
			Scribe_Values.Look(ref caravanSpecializationsMin, "CaravanSpecializationsMin", 1);
			Scribe_Values.Look(ref caravanSpecializationsMax, "CaravanSpecializationsMax", 1);
			_values.CaravanSpecializations = new IntRange(caravanSpecializationsMin, caravanSpecializationsMax);

			var orbitalSpecializationsMin = _values.OrbitalSpecializations.min;
			var orbitalSpecializationsMax = _values.OrbitalSpecializations.max;
			Scribe_Values.Look(ref orbitalSpecializationsMin, "OrbitalSpecializationsMin", 1);
			Scribe_Values.Look(ref orbitalSpecializationsMax, "OrbitalSpecializationsMax", 1);
			_values.OrbitalSpecializations = new IntRange(orbitalSpecializationsMin, orbitalSpecializationsMax);

			if (ModsConfig.IdeologyActive)
			{
				Scribe_Values.Look(ref _values.IdeologyMayAddStock, "IdeologyMayAddStock", true);
				Scribe_Values.Look(ref _values.IdeologyMayForbidTrading, "IdeologyMayForbidTrading", true);
			}

			Scribe_Values.Look(ref _values.IgnoreColonyPopulationCommonality, "IgnoreColonyPopulationCommonality");

			Scribe_Values.Look(ref _values.LogGen, "LogGen");
			Scribe_Values.Look(ref _values.LogStockGen, "LogStockGen");
		}
	}
}