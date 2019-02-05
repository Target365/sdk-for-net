using System;
using System.Runtime.Serialization;

namespace Target365.Sdk
{
	/// <summary>
	/// One-time password.
	/// </summary>
	[DataContract(Namespace = "http://schemas.target365.io/core")]
	public class OneTimePassword
	{
		/// <summary>
		/// Transaction id.
		/// </summary>
		[DataMember]
		public string TransactionId { get; set; }

		/// <summary>
		/// merchant id.
		/// </summary>
		[DataMember]
		public string MerchantId { get; set; }

		/// <summary>
		/// Recipient msisdn.
		/// </summary>
		[DataMember]
		public string Recipient { get; set; }

		/// <summary>
		/// Whether one-time password is for recurring payment.
		/// </summary>
		[DataMember]
		public bool Recurring { get; set; }

		/// <summary>
		/// One-time password SMS message sender (originator).
		/// </summary>
		[DataMember]
		public string Sender { get; set; }

		/// <summary>
		/// One-time password SMS message text.
		/// </summary>
		[DataMember]
		public string Message { get; set; }

		/// <summary>
		/// Time-To-Live (TTL) in minutes. Must be between 1 and 1440. Default value is 2.
		/// </summary>
		[DataMember]
		public int TimeToLive { get; set; } = 2;

		/// <summary>
		/// Whether one-time password sms has been delivered. Null means unknown.
		/// </summary>
		[DataMember]
		public bool? Delivered { get; set; }
	}
}