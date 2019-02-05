using System.Runtime.Serialization;

namespace Target365.Sdk
{
	/// <summary>
	/// Strex data.
	/// </summary>
	[DataContract(Namespace = "http://schemas.target365.io/core")]
	public class StrexData
	{
		/// <summary>
		/// merchant id.
		/// </summary>
		[DataMember]
		public string MerchantId { get; set; }

		/// <summary>
		/// Service code.
		/// </summary>
		[DataMember]
		public string ServiceCode { get; set; }

		/// <summary>
		/// Invoice text.
		/// </summary>
		[DataMember]
		public string InvoiceText { get; set; }

		/// <summary>
		/// Price.
		/// </summary>
		[DataMember]
		public decimal Price { get; set; }

		/// <summary>
		/// Read-only: Whether billing was performed. Null if status is unknown.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public bool? Billed { get; set; }
	}
}