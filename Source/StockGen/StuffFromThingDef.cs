using System.Collections.Generic;
using System.Text;
using RimWorld;
using Verse;

namespace TraderGen.StockGen
{
	/// <summary>
	/// Generates stock created using a single ThingDef as stuff. This ThingDef is chosen from a list.
	/// </summary>
	public class StuffFromThingDef : FromStuff
	{
		public List<ThingDef> availableThingDefs = new List<ThingDef>();

		public override void ConditionToText(ref StringBuilder b)
		{
			base.ConditionToText(ref b);
			Util.ToText(ref b, "availableThingDefs", availableThingDefs);
		}

		public override IEnumerable<string> ConfigErrors(TraderKindDef parentDef)
		{
			foreach (var err in base.ConfigErrors(parentDef))
			{
				yield return err;
			}

			if (availableThingDefs.Count == 0)
			{
				yield return "TraderGen.StockGen.StuffFromThingDef: availableThingDefs must not be empty.";
			}
		}

		protected override void SetStuffDef()
		{
			_stuffDef = availableThingDefs.RandomElement();
		}
	}
}