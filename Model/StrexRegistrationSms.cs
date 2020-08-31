using System.Runtime.Serialization;

namespace Target365.Sdk
{
	/// <summary>
	/// Strex registration SMS.
	/// </summary>
	[DataContract(Namespace = "http://schemas.target365.io/core")]
	public class StrexRegistrationSms
	{
		/// <summary>
		/// Merchant Id.
		/// </summary>
		[DataMember]
		public string MerchantId { get; set; }

		/// <summary>
		/// Transaction Id.
		/// </summary>
		[DataMember]
		public string TransactionId { get; set; }

		/// <summary>
		/// Recipient mobile number.
		/// </summary>
		[DataMember]
		public string Recipient { get; set; }

		/// <summary>
		/// SMS text to be added in the registration SMS. Registration URL will be added by Strex.
		/// </summary>
		[DataMember]
		public string SmsText { get; set; }
	}
}