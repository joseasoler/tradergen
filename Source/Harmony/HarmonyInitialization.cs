using System.Reflection;
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
		/// Initialization of the Harmony patching of the mod.
		/// </summary>
		static HarmonyInitialization()
		{
			// Initialize state.
			var harmony = new HarmonyLib.Harmony("joseasoler.TraderGen");
			// Annotation patches.
			harmony.PatchAll(Assembly.GetExecutingAssembly());
			// Patch vanilla code.
			TraderStock.Patch(harmony);
		}
	}
}