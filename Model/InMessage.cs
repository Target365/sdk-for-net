using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Target365.Sdk
{
	/// <summary>
	/// In-message.
	/// </summary>
	[DataContract(Namespace = "http://schemas.target365.io/core")]
	public class InMessage
	{
		/// <summary>
		/// Operator transaction id.
		/// </summary>
		[DataMember]
		public string TransactionId { get; set; }

		/// <summary>
		/// Keyword id associated with message. Can be null.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public string KeywordId { get; set; }

		/// <summary>
		/// Sender. Can be alphanumeric a phone number or the actual short number.
		/// </summary>
		[DataMember(IsRequired = true)]
		public string Sender { get; set; }

		/// <summary>
		/// Recipient. Phone number format includes a leading pluss and country code such as '+4798079008'.
		/// </summary>
		[DataMember(IsRequired = true)]
		public string Recipient { get; set; }

		/// <summary>
		/// Content. The actual text message content.
		/// </summary>
		[DataMember(IsRequired = true)]
		public string Content { get; set; }

		/// <summary>
		/// Whether the incoming message must be treated as a stop and unsubscribe request message.
		/// </summary>
		[DataMember(IsRequired = true)]
		public bool IsStopMessage { get; set; }

		/// <summary>
		/// Process attempts.
		/// </summary>
		[DataMember(IsRequired = true)]
		public int ProcessAttempts { get; set; }

		/// <summary>
		/// Whether message has been processed.
		/// </summary>
		[DataMember]
		public bool? Processed { get; set; }

		/// <summary>
		/// Creation time.
		/// </summary>
		[DataMember(IsRequired = true)]
		public DateTimeOffset Created { get; set; }

		/// <summary>
		/// Tags associated with keyword. Can be used for statistics and grouping.
		/// </summary>
		[DataMember]
		public ICollection<string> Tags { get; set; } = new HashSet<string>();

		/// <summary>
		/// Custom properties associated with message propagated from keyword.
		/// </summary>
		[DataMember]
		public Dictionary<string, object> Properties { get; set; }
	}
}
