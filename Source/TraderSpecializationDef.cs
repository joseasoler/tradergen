using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace TraderGen
{
	/// <summary>
	/// Defines a trader specialization. May also be used for categories.
	/// </summary>
	public class TraderSpecializationDef : Def
	{
		/// <summary>
		/// Stock generators added by this specialization.
		/// </summary>
		public List<StockGenerator> stockGens = new List<StockGenerator>();

		public override IEnumerable<string> ConfigErrors()
		{
			foreach (var error in base.ConfigErrors())
			{
				yield return error;
			}

			for (int index = 0; index < stockGens.Count; ++index)
			{
				List<string> errorList = null;
				try
				{
					IEnumerable<string> errorEnumerable = stockGens[index].ConfigErrors(null);
					errorList = errorEnumerable.ToList();
				}
				catch (Exception exception)
				{
					errorList = new List<string>
					{
						$"{Logger.Prefix} {defName} could not process config errors of the stock generator in position {index} due to an exception:",
						$"{exception}"
					};
				}

				foreach (var error in errorList)
				{
					yield return error;
				}
			}
		}
	}
}