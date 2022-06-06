using System.Collections.Generic;
using Verse;

namespace TG.StockRule
{
	/// <summary>
	/// All traders following an ideology with this rule will refuse to trade or stock the items returned by it.
	/// </summary>
	public abstract class Rule
	{
		/// <summary>
		/// Displays all XML configuration errors.
		/// </summary>
		/// <returns>Errors triggered by the data contained in this structure.</returns>
		public virtual IEnumerable<string> ConfigErrors()
		{
			yield break;
		}

		/// <summary>
		/// Items that this rule forbids traders from trading.
		/// </summary>
		/// <param name="def">Item to check.</param>
		/// <returns>True if the rule forbids trading this item.</returns>
		public virtual bool ForbidsTrading(in ThingDef def)
		{
			return false;
		}

		/// <summary>
		/// Items that this rule forbids traders from having in stock.
		/// </summary>
		/// <param name="def">Item to check.</param>
		/// <returns>True if the rule does not impede having this item in stock.</returns>
		public virtual bool ForbidsStocking(in ThingDef def)
		{
			return false;
		}
	}
}