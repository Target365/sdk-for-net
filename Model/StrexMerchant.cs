using System.Runtime.Serialization;

namespace Target365.Sdk
{
	/// <summary>
	/// Strex merchant.
	/// </summary>
	[DataContract(Namespace = "http://schemas.target365.io/core")]
	public class StrexMerchant
	{
		/// <summary>
		/// Merchant id.
		/// </summary>
		[DataMember]
		public string MerchantId { get; set; }

		/// <summary>
		/// Short number.
		/// </summary>
		[DataMember]
		public string ShortNumberId { get; set; }

		/// <summary>
		/// This is a write-only property and will always return null.
		/// </summary>
		[DataMember]
		public string Password { get; set; }
	}
}
