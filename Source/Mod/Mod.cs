using System;
using TG.Mod;
using TraderGen.TraderKind;
using UnityEngine;
using Verse;

namespace TraderGen.Mod
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

			// Disable caravan specializations for now.
			/*
			var caravanSpecializations = Settings.CaravanSpecializations;
			listing.Label("TG_CaravanSpecializations".Translate(), -1, "TG_CaravanSpecializationsTooltip".Translate());
			listing.IntRange(ref caravanSpecializations, 0, Settings.MaxSpecializations);
			Settings.CaravanSpecializations = caravanSpecializations;
			*/

			var orbitalSpecializations = Settings.OrbitalSpecializations;
			listing.Label("TG_OrbitalSpecializations".Translate(), -1, "TG_OrbitalSpecializationsTooltip".Translate());
			listing.IntRange(ref orbitalSpecializations, 0, Settings.MaxSpecializations);
			Settings.OrbitalSpecializations = orbitalSpecializations;

			if (ModsConfig.IdeologyActive)
			{
				listing.Gap();
				var mayAdd = Settings.IdeologyMayAddStock;
				listing.CheckboxLabeled("TG_IdeologyMayAddStock".Translate(), ref mayAdd,
					"TG_IdeologyMayAddStockTooltip".Translate());
				Settings.IdeologyMayAddStock = mayAdd;

				listing.Gap();
				var mayForbid = Settings.IdeologyMayForbidTrading;
				listing.CheckboxLabeled("TG_IdeologyMayForbidTrading".Translate(), ref mayForbid,
					"TG_IdeologyMayForbidTradingTooltip".Translate());
				Settings.IdeologyMayForbidTrading = mayForbid;
			}

			listing.Gap();
			var ignoreColonyPopulationCommonality = Settings.IgnoreColonyPopulationCommonality;
			listing.CheckboxLabeled("TG_IgnoreColonyPopulationCommonality".Translate(), ref ignoreColonyPopulationCommonality,
				"TG_IgnoreColonyPopulationCommonalityTooltip".Translate());
			Settings.IgnoreColonyPopulationCommonality = ignoreColonyPopulationCommonality;

			if (ModsConfig.RoyaltyActive)
			{
				listing.Gap();
				var sellPsylinkNeuroformers = Settings.SellPsylinkNeuroformers;
				listing.CheckboxLabeled("TG_SellPsylinkNeuroformers".Translate(), ref sellPsylinkNeuroformers,
					"TG_SellPsylinkNeuroformersTooltip".Translate());
				Settings.SellPsylinkNeuroformers = sellPsylinkNeuroformers;
			}

			listing.Gap();
			var resetButtonRect = listing.GetRect(30f);
			var resetWidth = resetButtonRect.width;
			resetButtonRect.width /= 5f;
			resetButtonRect.x += resetWidth - resetButtonRect.width;
			if (Widgets.ButtonText(resetButtonRect, "TG_ResetSettings".Translate()))
			{
				Settings.Reset();
			}

			TooltipHandler.TipRegion(resetButtonRect, "TG_ResetSettingsTooltip".Translate());
			listing.Gap(listing.verticalSpacing);

			if (Prefs.DevMode)
			{
				listing.GapLine(24f);
				listing.Gap();
				listing.Label("TG_DevelopmentOptions".Translate(), -1f, "TG_DevelopmentOptionsTooltip".Translate());
				listing.Gap();

				var logGen = Settings.LogGen;
				listing.CheckboxLabeled("TG_LogTraderGen".Translate(), ref logGen,
					"TG_LogTraderGenTooltip".Translate());
				Settings.LogGen = logGen;

				var logStockGen = Settings.LogStockGen;
				listing.CheckboxLabeled("TG_LogTraderStockGen".Translate(), ref logStockGen,
					"TG_LogTraderStockGenTooltip".Translate());
				Settings.LogStockGen = logStockGen;
			}


			listing.End();
			base.DoSettingsWindowContents(inRect);
		}
	}
}