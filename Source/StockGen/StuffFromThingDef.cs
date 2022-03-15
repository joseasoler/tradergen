using System.Text;
using Verse;

namespace TG.StockGen
{
	/// <summary>
	/// Generates stock created using a specific ThingDef as stuff.
	/// </summary>
	public class StuffFromThingDef : FromStuff
	{
		public ThingDef thingDef;

		public override void ConditionToText(ref StringBuilder b)
		{
			base.ConditionToText(ref b);
			b.Append($"thingDef: {thingDef}\n");
		}

		protected override void SetStuffDef()
		{
			_stuffDef = thingDef;
		}
	}
}