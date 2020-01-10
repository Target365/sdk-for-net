namespace Target365.Sdk
{
	/// <summary>
	/// Detailed status codes.
	/// </summary>
	public static class DetailedStatusCodes
	{
		/// <summary>
		/// Message has no status.
		/// </summary>
		public const string None = "None";
		
		/// <summary>
		/// Message is delivered to destination.
		/// </summary>
		public const string Delivered = "Delivered";

		/// <summary>
		/// Message validity period has expired.
		/// </summary>
		public const string Expired = "Expired";

		/// <summary>
		/// Message is undeliverable.
		/// </summary>
		public const string Undelivered = "Undelivered";

		/// <summary>
		/// Message is in invalid state.
		/// </summary>
		public const string UnknownError = "UnknownError";

		/// <summary>
		/// Message is in a rejected state.
		/// </summary>
		public const string Rejected = "Rejected";

		/// <summary>
		/// Unknown subscriber.
		/// </summary>
		public const string UnknownSubscriber = "UnknownSubscriber";

		/// <summary>
		/// Subscriber unavailable.
		/// </summary>
		public const string SubscriberUnavailable = "SubscriberUnavailable";

		/// <summary>
		/// Subscriber barred.
		/// </summary>
		public const string SubscriberBarred = "SubscriberBarred";

		/// <summary>
		/// Insufficient funds.
		/// </summary>
		public const string InsufficientFunds = "InsufficientFunds";

		/// <summary>
		/// Registration required.
		/// </summary>
		public const string RegistrationRequired = "RegistrationRequired";

		/// <summary>
		/// Unknown age.
		/// </summary>
		public const string UnknownAge = "UnknownAge";

		/// <summary>
		/// Duplicate transaction.
		/// </summary>
		public const string DuplicateTransaction = "DuplicateTransaction";

		/// <summary>
		/// Subscriber limit exceeded.
		/// </summary>
		public const string SubscriberLimitExceeded = "SubscriberLimitExceeded";

		/// <summary>
		/// Max pin retry reached.
		/// </summary>
		public const string MaxPinRetry = "MaxPinRetry";

		/// <summary>
		/// Invalid amount.
		/// </summary>
		public const string InvalidAmount = "InvalidAmount";

		/// <summary>
		/// One-time password expired.
		/// </summary>
		public const string OneTimePasswordExpired = "OneTimePasswordExpired";

		/// <summary>
		/// One-time password failed.
		/// </summary>
		public const string OneTimePasswordFailed = "OneTimePasswordFailed";

		/// <summary>
		/// Subscriber too young.
		/// </summary>
		public const string SubscriberTooYoung = "SubscriberTooYoung";

		/// <summary>
		/// Timeout error.
		/// </summary>
		public const string TimeoutError = "TimeoutError";
	}
}