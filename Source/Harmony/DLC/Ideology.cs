using System.Reflection;
using HarmonyLib;
using TraderGen.Ideo;
using Verse;

namespace TraderGen.Harmony.DLC
{
	/// <summary>
	/// Apply the Harmony patches for Ideology compatibility.
	/// </summary>
	public static class Ideology
	{
		/// <summary>
		/// Apply the Harmony patches for Ideology compatibility.
		/// </summary>
		/// <param name="harmony">Harmony library instance.</param>
		public static void Patch(HarmonyLib.Harmony harmony)
		{
			if (!ModsConfig.IdeologyActive) return;

			MethodInfo recachePrecepts =
				AccessTools.Method(typeof(RimWorld.Ideo), nameof(RimWorld.Ideo.RecachePrecepts));
			HarmonyMethod invalidateIdeoCache =
				new HarmonyMethod(AccessTools.Method(typeof(Ideology), nameof(InvalidateIdeoCache)));
			harmony.Patch(recachePrecepts, postfix: invalidateIdeoCache);
		}

		/// <summary>
		/// Invalidate the ideology trading cache after precepts have been recached.
		/// </summary>
		private static void InvalidateIdeoCache(ref RimWorld.Ideo __instance)
		{
			IdeoCache.Invalidate(__instance);
		}
	}
}