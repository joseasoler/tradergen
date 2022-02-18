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
			var listing = new Listing_Standard();
			listing.Begin(inRect);

			listing.Label("TG_MaxOrbitalShips".Translate(Settings.MaxOrbitalShips), -1,
				"TG_MaxOrbitalShipsTooltip".Translate());
			Settings.MaxOrbitalShips = (int) listing.Slider(Settings.MaxOrbitalShips, 1.0f, 10.0f);

			listing.Label("TG_SilverStockOrbitalTrader".Translate((int) Settings.OrbitalSilverScaling), -1,
				"TG_SilverStockOrbitalTraderTooltip".Translate());
			Settings.OrbitalSilverScaling = listing.Slider(Settings.OrbitalSilverScaling, Settings.MinSilverScaling,
				Settings.MaxSilverScaling);

			var resetButtonRect = listing.GetRect(30f);
			var width = resetButtonRect.width;
			resetButtonRect.width /= 5f;
			resetButtonRect.x += width - resetButtonRect.width;
			if (Widgets.ButtonText(resetButtonRect, "TG_ResetSettings".Translate()))
			{
				Settings.Reset();
			}

			TooltipHandler.TipRegion(resetButtonRect, "TG_ResetSettingsTooltip".Translate());
			listing.Gap(listing.verticalSpacing);

			listing.GapLine(24f);
			listing.Gap();
			listing.Label("TG_DevelopmentOptions".Translate(), -1f, "TG_DevelopmentOptionsTooltip".Translate());
			listing.Gap();

			var logGen = Settings.LogGen;
			listing.CheckboxLabeled("TG_LogTraderGen".Translate(), ref logGen,
				"TG_LogTraderGenTooltip".Translate());
			Settings.LogGen = logGen;

			listing.End();
			base.DoSettingsWindowContents(inRect);
		}
	}
}