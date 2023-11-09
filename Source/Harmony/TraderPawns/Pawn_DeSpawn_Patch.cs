using TraderGen.TraderKind;
using Verse;

namespace TraderGen.Harmony.TraderPawns
{
	/// <summary>
	/// Remove trader information of the pawn from the cache.
	/// </summary>
	internal static class Pawn_DeSpawn_Patch
	{
		private static void Prefix(Pawn __instance)
		{
			if (__instance.trader != null)
			{
				Cache.Remove(__instance);
			}
		}
	}
}