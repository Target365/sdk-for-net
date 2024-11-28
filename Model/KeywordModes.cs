using System;

namespace Target365.Sdk
{
	/// <summary>
	/// Keyword modes.
	/// </summary>
	public static class KeywordModes
	{		
		/// <summary>
		/// Deprecated - use <see cref="Startswith"/> instead.
		/// </summary>
		[Obsolete("Text is obsolete - please use Startswith instead", false)]
		public const string Text = "Text";

		/// <summary>
		/// Matching start of text.
		/// </summary>
		public const string Startswith = "Startswith";

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
