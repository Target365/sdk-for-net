using System.Collections.Generic;
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

		/// <summary>
		/// Max attempts, 1-5 (3 is default).
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public int? MaxAttempts { get; set; } = 3;

		/// <summary>
		/// Verification mode. AtLeastOnce is default and allows for idempotent verification. AtMostOnce only allows for exactly one verification.
		/// </summary>
		[DataMember]
		public DeliveryModes VerificationMode { get; set; } = DeliveryModes.AtLeastOnce;

		/// <summary>
		/// Tags associated with the generated out-message. Can be used for statistics and grouping.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public ICollection<string> Tags { get; set; } = new HashSet<string>();
	}
}
