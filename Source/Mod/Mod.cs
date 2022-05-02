using System;
using RimWorld;
using TG.TraderKind;
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

			var labelValue = Settings.PeriodOrbital != Settings.DisablePeriodOrbital
				? ((int) Settings.PeriodOrbital * GenDate.TicksPerDay).ToStringTicksToPeriodVerbose()
				: (string) "TG_ModifyPeriodOrbitalDoNotChange".Translate();

			var label = Settings.PeriodOrbital != 15U ? "TG_ModifyPeriodOrbital" : "TG_ModifyPeriodOrbitalDefault";

			listing.Label(label.Translate(labelValue), -1f, "TG_ModifyPeriodOrbitalTooltip".Translate());
			Settings.PeriodOrbital = (uint) Widgets.HorizontalSlider(listing.GetRect(22f), Settings.PeriodOrbital,
				Settings.DisablePeriodOrbital, Settings.MaxPeriodOrbital, false, null, null, null, 1.0f);
			listing.Gap(listing.verticalSpacing);

			foreach (var categoryObj in Enum.GetValues(typeof(TraderKindCategory)))
			{
				var category = (TraderKindCategory) categoryObj;
				if (category == TraderKindCategory.None)
				{
					continue;
				}

				var categoryName = Enum.GetName(typeof(TraderKindCategory), category);

				listing.Label($"TG_SilverStock{categoryName}".Translate((int) Settings.GetSilverScaling(category)), -1,
					$"TG_SilverStock{categoryName}Tooltip".Translate());
				var silverScaling = listing.Slider(Settings.GetSilverScaling(category), Settings.MinSilverScaling,
					Settings.MaxSilverScaling);
				Settings.SetSilverScaling(category, silverScaling);
			}

			var orbitalSpecializations = Settings.OrbitalSpecializations;
			listing.Label("TG_OrbitalSpecializations".Translate(), -1, "TG_OrbitalSpecializationsTooltip".Translate());
			listing.IntRange(ref orbitalSpecializations, 0, Settings.MaxOrbitalSpecializations);
			Settings.OrbitalSpecializations = orbitalSpecializations;

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

			listing.End();
			base.DoSettingsWindowContents(inRect);
		}

		public override void WriteSettings()
		{
			base.WriteSettings();
			DefPatcher.Patch();
		}
	}
}