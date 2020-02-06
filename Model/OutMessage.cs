using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Target365.Sdk
{
	/// <summary>
	/// Out-message.
	/// </summary>
	[DataContract(Namespace = "http://schemas.target365.io/core")]
	public class OutMessage
	{
		/// <summary>
		/// Transaction id. Must be unique per message if used. This can be used for guarding against resending messages.
		/// </summary>
		[DataMember(IsRequired = true)]
		public string TransactionId { get; set; }

		/// <summary>
		/// Session id. This can be used as the clients to get all out-messages associated to a specific session.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public string SessionId { get; set; }

		/// <summary>
		/// Correlation id. This can be used as the clients correlation id for tracking messages and delivery reports.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public string CorrelationId { get; set; }

		/// <summary>
		/// Keyword id associated with message. Can be null.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public string KeywordId { get; set; }

		/// <summary>
		/// Sender. Can be an alphanumeric string, a phone number or a short number.
		/// </summary>
		[DataMember(IsRequired = true)]
		public string Sender { get; set; }

		/// <summary>
		/// Recipient phone number.
		/// </summary>
		[DataMember(IsRequired = true)]
		public string Recipient { get; set; }

		/// <summary>
		/// Content. The actual text message content.
		/// </summary>
		[DataMember]
		public string Content { get; set; }

		/// <summary>
		/// Send time, in UTC. If omitted the send time is set to ASAP.
		/// </summary>
		[DataMember]
		public DateTimeOffset SendTime { get; set; }

		/// <summary>
		/// Message Time-To-Live (TTL) in minutes. Must be between 5 and 1440. Default value is 120.
		/// </summary>
		[DataMember]
		public int TimeToLive { get; set; } = 120;

		/// <summary>
		/// Priority. Can be 'Low', 'Normal' or 'High'. If omitted, default value is 'Normal'.
		/// </summary>
		[DataMember]
		public string Priority { get; set; } = MessagePriorities.Normal;

		/// <summary>
		/// Message delivery mode. Can be either AtLeastOnce or AtMostOnce. If omitted, default value is AtMostOnce.
		/// </summary>
		[DataMember]
		public DeliveryModes DeliveryMode { get; set; } = DeliveryModes.AtMostOnce;

		/// <summary>
		/// Delivery report url.
		/// </summary>
		[DataMember]
		public string DeliveryReportUrl { get; set; }

		/// <summary>
		/// Last modified time.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public DateTimeOffset LastModified { get; set; }

		/// <summary>
		/// Created time.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public DateTimeOffset Created { get; set; }

		/// <summary>
		/// Overall status code. See <see cref="StatusCodes"/>.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public string StatusCode { get; set; }

		/// <summary>
		/// Detailed status code. See <see cref="DetailedStatusCodes"/>.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public string DetailedStatusCode { get; set; }

		/// <summary>
		/// Whether message was delivered. Null if status is unknown.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public bool? Delivered { get; set; }

		/// <summary>
		/// Operator id (from delivery report).
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public string OperatorId { get; set; }

		/// <summary>
		/// Strex data associated with the out-message.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public StrexData Strex { get; set; }

		/// <summary>
		/// Whether Unicode message is allowed.
		/// True forces unicode SMS if Content is unicode.
		/// False forces message to fail if Content is unicode.
		/// Null auto-converts all unicode characters to ? and sends regular SMS.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public bool? AllowUnicode { get; set; }

		/// <summary>
		/// External SMSC transaction id.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public string SmscTransactionId { get; set; }

		/// <summary>
		/// SMSC message parts.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public int SmscMessageParts { get; set; }

		/// <summary>
		/// Tags associated with message. Can be used for statistics and grouping.
		/// </summary>
		[DataMember]
		public ICollection<string> Tags { get; set; } = new HashSet<string>();

		/// <summary>
		/// Custom properties associated with the out-message.
		/// </summary>
		[DataMember]
		public IDictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();
	}
}
