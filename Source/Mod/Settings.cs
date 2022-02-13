using System.Collections.Generic;
using Verse;

namespace TG.Mod
{
	/// <summary>
	/// Handles mod settings.
	/// </summary>
	public class Settings : ModSettings
	{
		/// <summary>
		/// Generate a detailed report of the orbital trade ship generation process and append it to the log.
		/// </summary>
		public static bool LogGen /* = false */;

		/// <summary>
		/// Silver stock of orbital traders in %.
		/// </summary>
		public static float OrbitalSilverScaling = 100.0f;

		/// <summary>
		/// Minimum allowed value for OrbitalSilverScaling.
		/// </summary>
		public const float MinSilverScaling = 5.0f;

		/// <summary>
		/// Maximum allowed value for OrbitalSilverScaling.
		/// </summary>
		public const float MaxSilverScaling = 500.0f;

		/// <summary>
		/// Save and load preferences.
		/// </summary>
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref LogGen, "LogGen");
			Scribe_Values.Look(ref OrbitalSilverScaling, "OrbitalSilverScaling");
		}
	}
}