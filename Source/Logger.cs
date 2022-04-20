using System.Collections.Generic;
using RimWorld;
using TG.Mod;
using Verse;

namespace TG
{
	/// <summary>
	/// Logs messages conditionally depending on mod settings and prepends a mod-specific prefix.
	/// </summary>
	public static class Logger
	{
		private const string Prefix = "[TraderGen] ";

		/// <summary>
		/// Logs trader procedural generation messages. 
		/// </summary>
		/// <param name="text">Text to be logged.</param>
		public static void Gen(string text)
		{
			if (Settings.LogGen)
			{
				Log.Message(Prefix + text);
			}
		}

		/// <summary>
		/// Logs an error on the terminal.
		/// </summary>
		/// <param name="text">Error text to log.</param>
		public static void Error(string text)
		{
			Log.Error(Prefix + text);
		}

		/// <summary>
		/// Logs an error exactly once.
		/// </summary>
		/// <param name="text">Error text to log.</param>
		public static void ErrorOnce(string text)
		{
			Log.ErrorOnce(text, text.GetHashCode());
		}

		public static void GeneratedThingsReport(in string traderName, in List<Thing> things)
		{
			if (!Settings.LogGen)
			{
				return;
			}

			var marketValue = 0.0f;
			var weight = 0.0f;
			var volume = 0.0f;
			foreach (var thing in things)
			{
				marketValue += thing.MarketValue * thing.stackCount;
				weight += thing.def.BaseMass * thing.stackCount;
				volume += thing.def.VolumePerUnit * thing.stackCount;
			}

			Gen($"{traderName} stock -> marketValue: {marketValue}, weight: {weight}, volume: {volume}");
		}
	}
}