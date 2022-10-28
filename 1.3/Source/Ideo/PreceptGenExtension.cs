using System.Collections.Generic;
using System.Linq;
using Verse;

namespace TG.Ideo
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

			foreach (var error in defs.SelectMany(def => def.ConfigErrors()))
			{
				yield return error;
			}
		}
	}
}