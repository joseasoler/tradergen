using Verse;

namespace TraderGen.StockGen
{
	/// <summary>
	/// Handles items such as elephant tusk or thrumbo horn, but also similar modded items such as gallatross horn.
	/// </summary>
	public class ExoticAnimalPart : ConditionMatcher
	{
		protected override bool CanBuy(in ThingDef def)
		{
			return def.thingSetMakerTags != null && def.thingSetMakerTags.Contains("AnimalPart") &&
			       Things.Util.IsExotic(def);
		}
	}
}