using System.Collections.Generic;
using Verse;

namespace TG.Ideo
{
	/// <summary>
	/// This mod extension associates an existing precept with the stock additions and rules it should apply.
	/// </summary>
	public class PreceptGenExtension : DefModExtension
	{
		public PreceptGenDef def;

		public override IEnumerable<string> ConfigErrors()
		{
			foreach (var error in base.ConfigErrors())
			{
				yield return error;
			}

			foreach (var error in def.ConfigErrors())
			{
				yield return error;
			}
		}
	}
}