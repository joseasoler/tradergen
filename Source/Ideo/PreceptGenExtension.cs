using System.Collections.Generic;
using Verse;

namespace TraderGen.Ideo
{
	/// <summary>
	/// This mod extension associates an existing precept with the stock additions and rules it should apply.
	/// </summary>
	public class PreceptGenExtension : DefModExtension
	{
		public List<PreceptGenDef> defs = new List<PreceptGenDef>();

		public override IEnumerable<string> ConfigErrors()
		{
			foreach (var error in base.ConfigErrors())
			{
				yield return error;
			}

			foreach (PreceptGenDef preceptGenDef in defs)
			{
				foreach (var error in preceptGenDef.ConfigErrors())
				{
					yield return error;
				}
			}
		}
	}
}