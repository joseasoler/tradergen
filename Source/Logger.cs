using System;
using System.Linq;
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
		/// Generates a short summary of a StockGenerator instance.
		/// </summary>
		/// <param name="generator">Instance to log.</param>
		public static string StockGen(in StockGenerator generator)
		{
			var text = generator.GetType().Name + '[';
			if (generator.GetType() == typeof(StockGenerator_SingleDef))
			{
				var g = (StockGenerator_SingleDef) generator;
				text += g.thingDef.defName ;
			}
			else if (generator.GetType() == typeof(StockGenerator_BuySingleDef))
			{
				var g = (StockGenerator_BuySingleDef) generator;
				text += g.thingDef.defName;
			}
			else if (generator.GetType() == typeof(StockGenerator_MultiDef))
			{
				var g = (StockGenerator_MultiDef) generator;
				text = g.thingDefs.Aggregate(text, (current, thingDef) => current + thingDef.defName + ',');
			}

			text += ']';
			
			if (generator.countRange.min != 0 || generator.countRange.max != 0)
			{
				text += ": " + generator.countRange;
			}

			return text;
		}

		public static void Error(string text)
		{
			Log.Error(Prefix + text);
		}
	}
}
