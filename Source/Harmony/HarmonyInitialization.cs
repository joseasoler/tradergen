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
			var harmony = new HarmonyLib.Harmony("joseasoler.TraderGen");
			harmony.PatchAll(Assembly.GetExecutingAssembly());
		}
	}
}