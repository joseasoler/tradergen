namespace TG
{
	/// <summary>
	/// Groups a link with its commonality. Used in the next attribute of LinkDefs.
	/// </summary>
	public class Link
	{
		/// <summary>
		/// Next LinkDef to follow.
		/// </summary>
		public LinkDef def;
	
		/// <summary>
		/// How common this LinkDef is in comparison with others in the same next list.
		/// </summary>
		public int commonality;
	}
}
