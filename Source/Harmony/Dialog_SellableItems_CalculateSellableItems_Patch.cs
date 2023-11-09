using HarmonyLib;
using RimWorld;

namespace TraderGen.Harmony
{
	/// <summary>
	/// Make TraderGen take over the calculation of sellable items.
	/// See Dialog_SellableItems_Constructor_Patch for details.
	/// </summary>
	[HarmonyPatch(typeof(Dialog_SellableItems), nameof(Dialog_SellableItems.CalculateSellableItems))]
	internal static class Dialog_SellableItems_CalculateSellableItems_Patch
	{
		private static bool Prefix()
		{
			return false;
		}
	}
}