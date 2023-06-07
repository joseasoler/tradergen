using System.Collections.Generic;
using RimWorld;
using Verse;

namespace TraderGen.StockGen
{
	/// <summary>
	/// Matches any resource which can be mined.
	/// </summary>
	public class MinableResource : ConditionMatcher
	{
		/// <summary>
		/// Minable resources and their precalculated commonality.
		/// </summary>
		private static Dictionary<ThingDef, float> _minableResources;

		/// <summary>
		/// Minable resources that will never be generated.
		/// </summary>
		public List<ThingDef> excludedThingDefs = new List<ThingDef>();

		private static void InitializeMinableResources()
		{
			if (_minableResources != null) return;

			_minableResources = new Dictionary<ThingDef, float>();

			foreach (var thingDef in DefDatabase<ThingDef>.AllDefsListForReading)
			{
				if (thingDef.building?.mineableThing != null)
				{
					// ToDo commonality
					_minableResources[thingDef.building.mineableThing] = 1.0f;
				}
			}
		}

		protected override float Weight(in ThingDef def, in int forTile, in Faction faction)
		{
			InitializeMinableResources();
			return _minableResources.TryGetValue(def, out var value) ? value : 0.0f;
		}

		protected override bool CanSell(in ThingDef def, in int forTile, in Faction faction)
		{
			InitializeMinableResources();
			return _minableResources.ContainsKey(def) && !excludedThingDefs.Contains(def);
		}

		protected override bool CanBuy(in ThingDef def)
		{
			InitializeMinableResources();
			return _minableResources.ContainsKey(def);
		}
	}
}