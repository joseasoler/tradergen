using System;
using LudeonTK;
using Verse;

namespace TraderGen.Debug
{
	public static class DebugActions
	{
		/// <summary>
		/// Category to use for TraderGen debug actions.
		/// </summary>
		public const string DebugCategory = "TraderGen";

		/// <summary>
		/// Sort DebugMenuOptions by label.
		/// </summary>
		/// <param name="a">First menu option.</param>
		/// <param name="b">Second menu option.</param>
		/// <returns></returns>
		public static int CompareDebugMenuOptions(DebugMenuOption a, DebugMenuOption b)
		{
			return string.Compare(a.label, b.label, StringComparison.Ordinal);
		}
	}
}