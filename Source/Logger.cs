using System;
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
		/// Lazily initialized reference to the settings of this mod.
		/// </summary>
		private static readonly Lazy<Settings> Settings =
			new Lazy<Settings>(LoadedModManager.GetMod<Mod.Mod>().GetSettings<Settings>);

		/// <summary>
		/// Logs trader procedural generation messages. 
		/// </summary>
		/// <param name="text">Text to be logged.</param>
		public static void Gen(string text)
		{
			if (Settings.Value.LogGen)
			{
				Log.Message(Prefix + text);
			}
		}

		public static void Error(string text)
		{
			Log.Error(Prefix + text);

		}
	}
}
