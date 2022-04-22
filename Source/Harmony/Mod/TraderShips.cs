using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using TG.TraderKind;
using Verse;

namespace TG.Harmony.Mod
{
	/// <summary>
	/// Compatibility with the Trader Ships mod.
	/// </summary>
	public static class TraderShips
	{
		/// <summary>
		/// Apply the Harmony patch for TraderShips compatibility only if the mod is loaded.
		/// </summary>
		/// <param name="harmony"></param>
		public static void Patch(HarmonyLib.Harmony harmony)
		{
			if (!LoadedModManager.RunningMods.Any(pack => pack.PackageId.Equals("automatic.traderships")))
			{
				return;
			}

			var exposeData = AccessTools.Method("TraderShips.LandedShip:ExposeData");
			var exposeDataPostfix = new HarmonyMethod(AccessTools.Method(typeof(TraderShips), nameof(ExposeDataPostfix)));
			harmony.Patch(exposeData, postfix: exposeDataPostfix);
		}

		/// <summary>
		/// LandedShips have their own map attribute.
		/// </summary>
		/// <param name="___def">TraderKindDef to generate.</param>
		/// <param name="___randomPriceFactorSeed">Random seed.</param>
		/// <param name="___map">Map in which the ship has landed.</param>
		/// <param name="___faction">Faction of the trader.</param>
		private static void ExposeDataPostfix(ref TraderKindDef ___def, int ___randomPriceFactorSeed, ref Map ___map,
			ref Faction ___faction)
		{
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				___def = Generator.Def(___def, ___randomPriceFactorSeed, ___map?.Tile ?? -1, ___faction);
			}
		}
	}
}