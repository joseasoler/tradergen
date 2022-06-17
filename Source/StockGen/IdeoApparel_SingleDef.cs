using System.Collections.Generic;
using System.Text;
using RimWorld;
using Verse;

namespace TG.StockGen
{
	public class IdeoApparel_SingleDef : IdeoColorApparel
	{
		/// <summary>
		/// Apparel def to generate.
		/// </summary>
		public ThingDef apparelDef;
		
		public IdeoApparel_SingleDef()
		{
			thingDefCountRange = IntRange.one;
		}

		public override void ConditionToText(ref StringBuilder b)
		{
			base.ConditionToText(ref b);
			b.Append($"apparelDef: {apparelDef}\n");
		}

		public override IEnumerable<string> ConfigErrors(TraderKindDef parentDef)
		{
			foreach (var err in base.ConfigErrors(parentDef))
			{
				yield return err;
			}

			if (apparelDef == null || !apparelDef.IsApparel)
			{
				yield return "TG.StockGen.IdeoApparel_SingleDef: Not associated with an apparel ThingDef.";
			}
		}

		protected override bool ApparelCondition(in ThingDef def)
		{
			return apparelDef == def;
		}
	}
}