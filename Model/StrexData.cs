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
		/// Business model.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public string BusinessModel { get; set; }

		/// <summary>
		/// Whether to use sms confirmation.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public bool? SmsConfirmation { get; set; }

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
		/// Whether billing was performed. Null if status is unknown.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public bool? Billed { get; set; }

		/// <summary>
		/// Strex result code.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public string ResultCode { get; set; }

		/// <summary>
		/// Strex result description.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public string ResultDescription { get; set; }
	}
}