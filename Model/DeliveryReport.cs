using System.Runtime.Serialization;

namespace Target365.Sdk
{
	/// <summary>
	/// Delivery report.
	/// </summary>
	[DataContract(Namespace = "http://schemas.target365.io/core")]
	public class DeliveryReport
	{
		/// <summary>
		/// Correlation id associated with the message.
		/// </summary>
		[DataMember]
		public string CorrelationId { get; set; }

		/// <summary>
		/// Transaction id associated with the message.
		/// </summary>
		[DataMember]
		public string TransactionId { get; set; }

		/// <summary>
		/// Price associated with the message.
		/// </summary>
		[DataMember]
		public decimal? Price { get; set; }

		/// <summary>
		/// Sender associated with the message.
		/// </summary>
		[DataMember(IsRequired = true)]
		public string Sender { get; set; }

		/// <summary>
		/// Recipient associated with the message.
		/// </summary>
		[DataMember(IsRequired = true)]
		public string Recipient { get; set; }

		/// <summary>
		/// Operator associated with the message. Can be 'telenor', 'netcom', 'ice' or 'networknorway'.
		/// </summary>
		[DataMember(IsRequired = true)]
		public string Operator { get; set; }

		/// <summary>
		/// Delivery status code.
		/// </summary>
		[DataMember]
		public string StatusCode { get; set; }

		/// <summary>
		/// Whether message was delivered. Null if status is unknown.
		/// </summary>
		[DataMember]
		public bool? Delivered { get; set; }

		/// <summary>
		/// Whether billing was performed. Null if status is unknown.
		/// </summary>
		[DataMember]
		public bool? Billed { get; set; }
	}
}
