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
		/// Transaction id associated with the message.
		/// </summary>
		[DataMember]
		public string TransactionId { get; set; }

		/// <summary>
		/// Correlation id associated with the message.
		/// </summary>
		[DataMember]
		public string CorrelationId { get; set; }

		/// <summary>
		/// Price associated with the message.
		/// </summary>
		[DataMember]
		public decimal? Price { get; set; }

		/// <summary>
		/// Sender associated with the message.
		/// </summary>
		[DataMember]
		public string Sender { get; set; }

		/// <summary>
		/// Recipient associated with the message.
		/// </summary>
		[DataMember]
		public string Recipient { get; set; }

		/// <summary>
		/// Operator id associated with the message. Can be 'no.telenor', 'no.telia', 'no.ice' etc.
		/// </summary>
		[DataMember]
		public string OperatorId { get; set; }

		/// <summary>
		/// Overall status code.
		/// </summary>
		[DataMember]
		public string StatusCode { get; set; }

		/// <summary>
		/// Detailed status code.
		/// </summary>
		[DataMember]
		public string DetailedStatusCode { get; set; }

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

		/// <summary>
		/// SMSC transaction id.
		/// </summary>
		[DataMember]
		public string SmscTransactionId { get; set; }

		/// <summary>
		/// SMSC message parts.
		/// </summary>
		[DataMember]
		public int SmscMessageParts { get; set; }
	}
}
