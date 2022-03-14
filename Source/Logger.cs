using System;
using System.Linq;
using RimWorld;
using TG.Mod;
using TG.StockGen;
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
	}
}