using System.Linq;
using Verse;

namespace TG.Harmony
{
	/// <summary>
	/// Utilities used by TraderGen harmony patching.
	/// </summary>
	public static class HarmonyUtils
	{
		// PackageIDs of the Trader Ships mod and its re-textures.
		private static readonly string[] TraderShipMods =
		{
			"automatic.traderships", // Trader Ships
			"steampunk.tradeships", // Steampunk: Trader Airships
			"rimeffect.themistraders" // Rim-Effect: Themis Traders
		};

		/// <summary>
		/// Lazily initialized value storing if Trader Ships or any of its re-textures are active.
		/// </summary>
		private static bool? _traderShipsEnabled;

		/// <summary>
		/// Returns true if the Trader Ships mod or any of its re-textures are being used.
		/// </summary>
		/// <returns>True if any of the mentioned mods are being used.</returns>
		public static bool TraderShipsEnabled()
		{
			if (_traderShipsEnabled == null)
			{
				_traderShipsEnabled = LoadedModManager.RunningMods.Any(pack => TraderShipMods.Contains(pack.PackageId));
			}

			return (bool) _traderShipsEnabled;
		}

		/// <summary>
		/// Lazily initialized value tracking if the Trade UI revised mod is active.
		/// </summary>
		private static bool? _tradeUIRevisedActive;

		/// <summary>
		/// Returns true if the Trade UI revised mod is being used.
		/// </summary>
		/// <returns>True if any of the mentioned mods are being used.</returns>
		public static bool TradeUIRevisedActive()
		{
			if (_tradeUIRevisedActive == null)
			{
				_tradeUIRevisedActive = LoadedModManager.RunningMods.Any(pack => pack.PackageId == "hobtook.tradeui");
			}

			return (bool) _tradeUIRevisedActive;
		}
	}
}