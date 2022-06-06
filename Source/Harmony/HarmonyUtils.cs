using System.Linq;
using Verse;

namespace TG.Harmony
{
	/// <summary>
	/// Utilities used by TraderGen harmony patching.
	/// </summary>
	public class HarmonyUtils
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
	}
}