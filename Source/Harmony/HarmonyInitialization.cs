using System.Reflection;
using HarmonyLib;
using Verse;

namespace TG.Harmony
{
	/// <summary>
	/// Initialization of the Harmony patching of the mod.
	/// </summary>
	[StaticConstructorOnStartup]
	public class HarmonyInitialization
	{
		/// <summary>
		/// True if the Trader Ships mod (id "automatic.traderships") is currently loaded.
		/// </summary>
		public static readonly bool TraderShipsModEnabled;

		/// <summary>
		/// Initialization of the Harmony patching of the mod.
		/// </summary>
		static HarmonyInitialization()
		{
			// Initialize state.
			TraderShipsModEnabled =
				LoadedModManager.RunningMods.FirstIndexOf(pack => pack.PackageId.Equals("automatic.traderships")) >= 0;
			var harmony = new HarmonyLib.Harmony("joseasoler.TraderGen");
			// Annotation patches.
			harmony.PatchAll(Assembly.GetExecutingAssembly());
			// Patch vanilla code.
			DebugActionAddTradeShip.Patch(harmony);
			TradeShip.Patch(harmony);
			// Patch mod code.
			Mod.TraderShips.Patch(harmony);
		}
	}
}