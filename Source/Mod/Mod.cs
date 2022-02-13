using UnityEngine;
using Verse;

namespace TG.Mod
{
	/// <summary>
	/// Loads mod settings and displays the mod settings window.
	/// </summary>
	public class Mod : Verse.Mod
	{
		/// <summary>
		/// Reads and initializes mod settings.
		/// </summary>
		/// <param name="content">Content pack data of this mod.</param>
		public Mod(ModContentPack content) : base(content)
		{
			GetSettings<Settings>();
		}

		/// <summary>
		/// Name of the mod in the settings list.
		/// </summary>
		/// <returns>Name of the mod in the settings list.</returns>
		public override string SettingsCategory()
		{
			return "TraderGen";
		}

		/// <summary>
		/// Contents of the mod settings window.
		/// </summary>
		/// <param name="inRect">Available area for drawing the settings.</param>
		public override void DoSettingsWindowContents(Rect inRect)
		{
			var listingStandard = new Listing_Standard();
			listingStandard.Begin(inRect);
			listingStandard.Label("TG_SilverStockOrbitalTrader".Translate((int) Settings.OrbitalSilverScaling), -1,
				"TG_SilverStockOrbitalTraderTooltip".Translate());
			
			Settings.OrbitalSilverScaling = listingStandard.Slider(Settings.OrbitalSilverScaling, Settings.MinSilverScaling,
				Settings.MaxSilverScaling);
			listingStandard.CheckboxLabeled("TG_LogTraderGen".Translate(), ref Settings.LogGen,
				"TG_LogTraderGenTooltip".Translate());
			listingStandard.End();
			base.DoSettingsWindowContents(inRect);
		}
	}
}