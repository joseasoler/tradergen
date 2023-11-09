using HarmonyLib;
using RimWorld;
using TraderGen.StockModification;
using Verse;

namespace TraderGen.Harmony
{
	[HarmonyPatch(typeof(CompQuality), nameof(CompQuality.PostPostGeneratedForTrader))]
	public static class CompQuality_PostPostGeneratedForTrader_Patch
	{
		internal static bool Prefix(CompQuality __instance, TraderKindDef trader, int forTile, Faction forFaction)
		{
			ChangeStockQuality.QualityRange qualityRange = ChangeStockQuality.Get();
			if (qualityRange == null)
			{
				return true;
			}

			var fromGaussian = Rand.Gaussian((float)qualityRange.CenterQuality + 0.5f, 2.5f);
			QualityCategory quality;
			if (fromGaussian < (float)qualityRange.MinQuality)
			{
				quality = qualityRange.MinQuality;
			}
			else if (fromGaussian > (float)qualityRange.MaxQuality)
			{
				quality = qualityRange.MaxQuality;
			}
			else
			{
				quality = (QualityCategory)(int)fromGaussian;
			}

			__instance.SetQuality(quality, ArtGenerationContext.Outsider);
			return false;
		}
	}
}