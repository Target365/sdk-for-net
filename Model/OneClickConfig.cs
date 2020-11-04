using System;
using System.Runtime.Serialization;

namespace Target365.Sdk
{
	/// <summary>
	/// One-click config.
	/// </summary>
	public class OneClickConfig
	{
		/// <summary>
		/// Unique config id.
		/// </summary>
		public string ConfigId { get; set; }

		/// <summary>
		/// Created time.
		/// </summary>
		[DataMember]
		public DateTimeOffset Created { get; set; }

		/// <summary>
		/// Last modified time.
		/// </summary>
		[DataMember]
		public DateTimeOffset LastModified { get; set; }

		/// <summary>
		/// Short number.
		/// </summary>
		[DataMember]
		public string ShortNumber { get; set; }

		/// <summary>
		/// Merchant id.
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
		[DataMember]
		public string BusinessModel { get; set; }

		/// <summary>
		/// Age requirements - typically 18 for subscriptions and adult content.
		/// </summary>
		[DataMember]
		public int Age { get; set; }

		/// <summary>
		/// Whether the transaction should be flagged as restricted.
		/// </summary>
		[DataMember]
		public bool IsRestricted { get; set; }

		/// <summary>
		/// Invoice text - Appears in the Strex portal available for end users.
		/// </summary>
		[DataMember]
		public string InvoiceText { get; set; }

		/// <summary>
		/// Price - in whole NOK. Cents (øre) are supported by the first two decimal places.
		/// </summary>
		[DataMember]
		public decimal Price { get; set; }

		/// <summary>
		/// Timeout in minutes for transactions which trigger end user registration. Default value is 5.
		/// </summary>
		[DataMember]
		public int Timeout { get; set; }

		/// <summary>
		/// Whether this config is for setting up subscriptions and recurring payments.
		/// </summary>
		[DataMember]
		public bool Recurring { get; set; }

		/// <summary>
		/// One-click redirect url.
		/// </summary>
		[DataMember]
		public string RedirectUrl { get; set; }

		/// <summary>
		/// One-click online text to use when oneclick msisdn detection is online and PIN-code can be skipped.
		/// </summary>
		[DataMember]
		public string OnlineText { get; set; }

		/// <summary>
		/// One-click text to use when oneclick msisdn detection is offline and PIN-code is used.
		/// </summary>
		[DataMember]
		public string OfflineText { get; set; }

		/// <summary>
		/// Subscription interval (weekly, monthly, yearly)
		/// </summary>
		[DataMember]
		public string SubscriptionInterval { get; set; }

		/// <summary>
		/// Subscription price - in whole NOK. Cents (øre) are supported by the first two decimal places.
		/// </summary>
		[DataMember]
		public decimal? SubscriptionPrice { get; set; }

		/// <summary>
		/// Subscription start sms - sent when recurring transaction started.
		/// </summary>
		[DataMember]
		public string SubscriptionStartSms { get; set; }
	}
}
