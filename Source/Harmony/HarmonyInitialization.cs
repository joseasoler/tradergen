using System.Reflection;
using TG.Harmony;
using Verse;

namespace TG.Harmony
{
	/// <summary>
	/// Initialization of the Harmony patching of the mod.
	/// </summary>
	[StaticConstructorOnStartup]
	// ReSharper disable once UnusedType.Global
	public class HarmonyInitialization
	{
		/// <summary>
		/// Initialization of the Harmony patching of the mod.
		/// </summary>
		static HarmonyInitialization()
		{
			var harmony = new HarmonyLib.Harmony("joseasoler.TraderGen");
			// Annotation patches.
			harmony.PatchAll(Assembly.GetExecutingAssembly());
			// Manual patching.
			TradeShip.Patch(harmony);
		}
	}
}