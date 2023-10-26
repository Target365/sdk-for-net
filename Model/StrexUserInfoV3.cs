using System.Runtime.Serialization;

namespace Target365.Sdk
{
	/// <summary>
	/// Strex user validity values.
	/// </summary>
	[DataContract(Namespace = "http://schemas.target365.io/core")]
	public class StrexUserInfoV3
	{
		/// <summary>
		/// Transid
		/// </summary>
		public string Transid { get; set; }

		/// <summary>
		/// Result: 0 - Ok
		/// </summary>
		[DataMember]
		public string Result { get; set; }

		/// <summary>
		/// Subscriber preferred source of funds: 1 - MNO bill, 2 – (not in use), 3 – payment card
		/// </summary>
		[DataMember]
		public string Preferred_sof { get; set; }

		/// <summary>
		/// Subscriber MNO subscription type: "prepaid" or "postpaid"
		/// </summary>
		[DataMember]
		public string Postpaid_or_prepaid { get; set; }

		/// <summary>
		/// Maximum amount (In øre) which can be charged in a single sell transaction
		/// </summary>
		[DataMember]
		public string Limit_trans { get; set; }

		/// <summary>
		/// Remaining monthly amount (in øre) which can be charged the subscriber before exceeding the monthly limit. "0" - limit already reached
		/// </summary>
		[DataMember]
		public string Remainder_month { get; set; }

		/// <summary>
		/// Remaining yearly amount (in øre) which can be charged the subscriber before exceeding the yearly limit. "0" - limit already reached, "-1" - no limit applicable, subscriber valid for purchases above defined yearly limit
		/// </summary>
		[DataMember]
		public string Remainder_year { get; set; }
	}
}