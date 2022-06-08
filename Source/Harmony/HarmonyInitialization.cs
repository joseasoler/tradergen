using System.Reflection;
using Verse;

namespace TG.Harmony
{
	/// <summary>
	/// Initialization of the Harmony patching of the mod.
	/// </summary>
	[StaticConstructorOnStartup]
	public static class HarmonyInitialization
	{
		/// <summary>
		/// Initialization of the Harmony patching of the mod.
		/// </summary>
		static HarmonyInitialization()
		{
			// Initialize state.
			var harmony = new HarmonyLib.Harmony("joseasoler.TraderGen");
			// Annotation patches.
			harmony.PatchAll(Assembly.GetExecutingAssembly());
			// Conditional patches.
			TradeShipGen.Patch(harmony);
			// DLC specific patches.
			DLC.Ideology.Patch(harmony);
			// Manual patching for other mods.
			Mod.TraderShips.Patch(harmony);
		}
	}
}