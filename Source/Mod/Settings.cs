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
		public bool LogGen /* = false */;

		/// <summary>
		/// Save and load preferences.
		/// </summary>
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref LogGen, "logGen");
		}
	}
}