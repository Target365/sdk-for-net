using System.Runtime.Serialization;

namespace Target365.Sdk
{
	/// <summary>
	/// Pincode.
	/// </summary>
	[DataContract(Namespace = "http://schemas.target365.io/core")]
	public class Pincode
	{
		/// <summary>
		/// TransactionId.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public string TransactionId { get; set; }

		/// <summary>
		/// Msisdn to receive pincode.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public string Recipient { get; set; }

		/// <summary>
		/// Sender of SMS.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public string Sender { get; set; }

		/// <summary>
		/// Text inserted before pincode (optional).
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public string PrefixText { get; set; }

		/// <summary>
		/// Text added after pincode (optional).
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public string SuffixText { get; set; }

		/// <summary>
		/// Length of pincode, 4-6 digits (optional).
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public int? PincodeLength { get; set; } = 4;
	}
}
