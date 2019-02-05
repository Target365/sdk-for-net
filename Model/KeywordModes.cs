namespace Target365.Sdk
{
	/// <summary>
	/// Keyword modes.
	/// </summary>
	public static class KeywordModes
	{
		/// <summary>
		/// Text mode with plain text matching.
		/// </summary>
		public const string Text = "Text";

		/// <summary>
		/// Wildcard mode with support for special characters like '*', '?', '#' etc.
		/// </summary>
		public const string Wildcard = "Wildcard";

		/// <summary>
		/// Regular expression mode (.NET style).
		/// </summary>
		public const string Regex = "Regex";
	}
}
