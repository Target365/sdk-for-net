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
		/// Invoice text - this shows up on the Strex portal for end users.
		/// </summary>
		[DataMember]
		public string InvoiceText { get; set; }

		/// <summary>
		/// Price - price to charge in whole NOK. Two decimals are supported (øre).
		/// </summary>
		[DataMember]
		public decimal Price { get; set; }

		/// <summary>
		/// Timeout in minutes for transactions which trigger end user registration. Default value is 5.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public int Timeout { get; set; } = 5;

		/// <summary>
		/// Service id used for pre-authorizations and recurring billing.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public string PreAuthServiceId { get; set; }

		/// <summary>
		/// Service description used for pre-authorizations and recurring billing.
		/// </summary>
		[DataMember]
		public string PreAuthServiceDescription { get; set; }

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