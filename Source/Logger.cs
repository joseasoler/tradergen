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
		/// Generates a short summary of a StockGenerator instance.
		/// </summary>
		/// <param name="generator">Instance to log.</param>
		public static string StockGen(in StockGenerator generator)
		{
			var text = generator.GetType().Name + '[';
			if (generator.GetType() == typeof(StockGenerator_SingleDef))
			{
				var g = (StockGenerator_SingleDef) generator;
				text += g.thingDef.defName;
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
			else if (generator.GetType() == typeof(StockGenerator_Tag))
			{
				var g = (StockGenerator_Tag) generator;
				text += g.tradeTag;
				text += " thingDefCountRange:{" + g.thingDefCountRange + "} ";
				if (g.excludedThingDefs != null && g.excludedThingDefs.Count > 0)
				{
					text += " except{";
					text += g.excludedThingDefs.Aggregate(text, (current, thingDef) => current + thingDef.defName + ',');
					text += '}';
				}
			}
			else if (generator.GetType() == typeof(StockGenerator_Animals))
			{
				var g = (StockGenerator_Animals) generator;
				if (g.tradeTagsBuy != null && g.tradeTagsBuy.Count > 0)
				{
					text += "buys{";
					text = g.tradeTagsBuy.Aggregate(text, (current, tag) => current + (tag + ','));
					text += "} ";
				}

				if (g.tradeTagsSell != null && g.tradeTagsSell.Count > 0)
				{
					text += "sells{";
					text = g.tradeTagsSell.Aggregate(text, (current, tag) => current + (tag + ','));
					text += "} ";
				}

				text += "kindCountRange:" + g.kindCountRange;
				text += " wildness:{" + g.minWildness + ", " + g.maxWildness + "} ";
				text += "checkTemperature: " + g.checkTemperature;
			}
			else if (generator.GetType().IsAssignableFrom(typeof(ConditionMatcher)))
			{
				var g = (ConditionMatcher) generator;
				text += " thingDefCountRange:{" + g.thingDefCountRange + '}';
			}

			text += ']';

			if (generator.countRange.min != 0 || generator.countRange.max != 0)
			{
				text += ": " + generator.countRange;
			}

			return text;
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