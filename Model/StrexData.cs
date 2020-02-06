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
		/// Merchant id - provided by Strex.
		/// </summary>
		[DataMember]
		public string MerchantId { get; set; }

		/// <summary>
		/// Service code - provided by Strex. See <see cref="ServiceCodes"/>.
		/// </summary>
		[DataMember]
		public string ServiceCode { get; set; }

		/// <summary>
		/// Business model - optional and provided by Strex.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public string BusinessModel { get; set; }

		/// <summary>
		/// Age requirements - typically 18 for subscriptions and adult content. Default value is 0.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public int Age { get; set; }

		/// <summary>
		/// Whether the transaction should be flagged as restricted - provided by Strex.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public bool IsRestricted { get; set; }

		/// <summary>
		/// Whether to use sms confirmation.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public bool? SmsConfirmation { get; set; }

		/// <summary>
		/// Invoice text - this shows up on the end users invoice from the mobile operator.
		/// </summary>
		[DataMember]
		public string InvoiceText { get; set; }

		/// <summary>
		/// Price - price to charge in whole NOK. Two decimals are supported (øre).
		/// </summary>
		[DataMember]
		public decimal Price { get; set; }

		/// <summary>
		/// Read-only: Whether billing was performed. Null if status is unknown.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public bool? Billed { get; set; }

		/// <summary>
		/// Read-only: Strex payment gateway result code.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public string ResultCode { get; set; }

		/// <summary>
		/// Read-only: Strex payment gateway result description.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public string ResultDescription { get; set; }
	}
}