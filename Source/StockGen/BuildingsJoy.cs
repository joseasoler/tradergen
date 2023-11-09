using System.Linq;
using Verse;
using ThingCategory = TraderGen.DefOfs.ThingCategory;

namespace TraderGen.StockGen
{
	/// <summary>
	/// Generates recreation buildings. When compared against the vanilla television approach, it also generates
	/// musical instruments.
	/// Unfortunately, BuildingsJoy buildings have their TechLevel set to undefined and therefore it is not possible
	/// to use it to filter lower tech buildings out. This StockGen uses the TechLevel of their research instead.
	/// </summary>
	public class BuildingsJoy : ConditionMatcher
	{
		protected override bool ValidTechLevel(in ThingDef def)
		{
			var techLevel = def.techLevel;
			if (def.researchPrerequisites != null && def.researchPrerequisites.Count > 0)
			{
				techLevel = def.researchPrerequisites.Max(researchProjectDef => researchProjectDef.techLevel);
			}

			return techLevel >= minTechLevelGenerate && techLevel <= maxTechLevelGenerate;
		}

		protected override bool CanBuy(in ThingDef def)
		{
			return ThingCategory.BuildingsJoy.DescendantThingDefs.Contains(def);
		}
	}
}