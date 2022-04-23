using System.Collections.Generic;
using RimWorld;
using Verse;

namespace TG.Mod
{
	/// <summary>
	/// Stores the onDays and offDays values of a StorytellerCompProperties_OnOffCycle.
	/// </summary>
	internal struct OrbitalArrivalValues
	{
		/// <summary>
		/// Number of days in which the event may randomly happen.
		/// </summary>
		internal readonly uint onDays;

		/// <summary>
		/// Number of days in which the event will not happen.
		/// </summary>
		internal readonly uint offDays;

		/// <summary>
		/// Construct the object from existing values to backup the default settings of a storyteller.
		/// </summary>
		/// <param name="on">Number of days in which the event may randomly happen.</param>
		/// <param name="off">Number of days in which the event will not happen.</param>
		public OrbitalArrivalValues(uint on, uint off)
		{
			onDays = on;
			offDays = off;
		}

		/// <summary>
		/// Calculates onDays and offDays from the value of the PeriodOrbital setting.
		/// onDays is equal to half of the period.
		/// offDays is equal to half of the period, plus one if the period was odd.
		/// When the period is exactly one, the general rule is not followed and onDays is 1 and offDays is 0 instead.
		/// </summary>
		/// <param name="periodOrbital">Total days of the period between orbital trader arrivals.</param>
		public OrbitalArrivalValues(uint periodOrbital)
		{
			onDays = periodOrbital > 1U ? periodOrbital / 2U : 1U;
			offDays = periodOrbital > 1U ? periodOrbital % 2 + periodOrbital / 2U : 0U;
		}
	}

	/// <summary>
	/// Responsible for patching StorytellerDefs after settings are changed.
	/// </summary>
	[StaticConstructorOnStartup]
	public static class DefPatcher
	{
		private static Dictionary<ushort, OrbitalArrivalValues> _originalOrbitalArrivalValues =
			new Dictionary<ushort, OrbitalArrivalValues>();

		static DefPatcher()
		{
			Patch();
		}

		public static void Patch()
		{
			var periodOrbitalDisabled = Settings.PeriodOrbital == Settings.DisablePeriodOrbital;

			// Default case, no patching is necessary.
			if (periodOrbitalDisabled && _originalOrbitalArrivalValues.Count == 0)
			{
				return;
			}

			// Will be overriden if periodOrbitalDisabled is true.
			var arrivalValues = new OrbitalArrivalValues(Settings.PeriodOrbital);

			foreach (var def in DefDatabase<StorytellerDef>.AllDefs)
			{
				foreach (var comp in def.comps)
				{
					if (comp.GetType() != typeof(StorytellerCompProperties_OnOffCycle)) continue;
					var onOffComp = (StorytellerCompProperties_OnOffCycle) comp;
					if (onOffComp.incident == null || onOffComp.incident != IncidentDefOf.OrbitalTraderArrival) continue;
					if (periodOrbitalDisabled)
					{
						if (!_originalOrbitalArrivalValues.ContainsKey(def.shortHash))
						{
							Logger.ErrorOnce($"Could not revert orbital trader arrival changes from {def.defName}");
							return;
						}

						arrivalValues = _originalOrbitalArrivalValues[def.shortHash];
					}
					else if (!_originalOrbitalArrivalValues.ContainsKey(def.shortHash))
					{
						_originalOrbitalArrivalValues[def.shortHash] = arrivalValues;
					}

					onOffComp.onDays = arrivalValues.onDays;
					onOffComp.offDays = arrivalValues.offDays;
					break;
				}
			}

			if (Find.Storyteller != null)
			{
				Find.Storyteller.Notify_DefChanged();
			}
		}
	}
}