namespace Target365.Sdk
{
	/// <summary>
	/// Out-message status codes.
	/// </summary>
	public static class OutMessageStatusCodes
	{
		/// <summary>
		/// Queued - message is queued.
		/// </summary>
		public const string Queued = "Queued";

		/// <summary>
		/// Sent - message has been sent.
		/// </summary>
		public const string Sent = "Sent";

		/// <summary>
		/// Failed - message has failed.
		/// </summary>
		public const string Failed = "Failed";

		/// <summary>
		/// OK - message has been delivered/billed.
		/// </summary>
		public const string Ok = "Ok";

		/// <summary>
		/// Reversed - message billing has been reversed.
		/// </summary>
		public const string Reversed = "Reversed";
	}
}