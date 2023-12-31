﻿using System;
using System.Reflection;
using TraderGen.Harmony.Caravans;
using Verse;

namespace TraderGen.Harmony
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
			ModAssemblies.Initialize();
			var harmony = new HarmonyLib.Harmony("joseasoler.TraderGen");
			// Annotation patches.
			harmony.PatchAll(Assembly.GetExecutingAssembly());
			// Conditional patches.
			TradeShipGen.Patch(harmony);
			// DLC specific patches.
			DLC.Ideology.Patch(harmony);
			// Manual patching for other mods.
			try
			{
				Mod.TraderShips.Patch(harmony);
			}
			catch (Exception exc)
			{
				Logger.Error("Could not apply patch for the Trader Ships mod:");
				Logger.Error(exc.ToString());
			}
			try
			{
				Mod.TradeUIRevised.Patch(harmony);
			}
			catch (Exception exc)
			{
				Logger.Error("Could not apply patch for the Trade UI Revised mod:");
				Logger.Error(exc.ToString());
			}
		}
	}
}