using Verse;

namespace TG.StockGen
{
	/// <summary>
	/// Generates stock created using a specific ThingDef as stuff.
	/// </summary>
	public class StuffFromThingDef : FromStuff
	{
		public ThingDef thingDef;

		protected override void SetStuffDef()
		{
			_stuffDef = thingDef;
		}
	}
}