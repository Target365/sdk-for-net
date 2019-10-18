using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Target365.Sdk
{
	/// <summary>
	/// Strex transaction.
	/// </summary>
	public class StrexTransaction : StrexData
	{
		/// <summary>
		/// Transaction id. Must be unique per transaction if used. This can be used for guarding against resending.
		/// </summary>
		[DataMember(IsRequired = true)]
		public string TransactionId { get; set; }

		/// <summary>
		/// Session id. This can be used as the clients relate transactions associated to a specific session.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public string SessionId { get; set; }

		/// <summary>
		/// Correlation id. This can be used as the clients correlation id for tracking transactions.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public string CorrelationId { get; set; }

		/// <summary>
		/// Short number.
		/// </summary>
		[DataMember(IsRequired = true)]
		public string ShortNumber { get; set; }

		/// <summary>
		/// Recipient phone number.
		/// </summary>
		[DataMember(IsRequired = true)]
		public string Recipient { get; set; }

		/// <summary>
		/// Optional SMS text message content (Not used for direct billing).
		/// </summary>
		[DataMember]
		public string Content { get; set; }

		/// <summary>
		/// One-Time-Password. Used for Strex one-time password verification.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public string OneTimePassword { get; set; }

		/// <summary>
		/// Delivery mode.
		/// </summary>
		[DataMember]
		public DeliveryModes DeliveryMode { get; set; } = DeliveryModes.AtMostOnce;

		/// <summary>
		/// Delivery status code.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public string StatusCode { get; set; }

		/// <summary>
		/// Created time.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public DateTimeOffset Created { get; set; }

		/// <summary>
		/// Last modified time.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public DateTimeOffset LastModified { get; set; }

		/// <summary>
		/// Tags associated with transaction. Can be used for statistics and grouping.
		/// </summary>
		[DataMember]
		public ICollection<string> Tags { get; set; } = new HashSet<string>();

		/// <summary>
		/// Custom properties associated with the transaction.
		/// </summary>
		[DataMember]
		public IDictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();
	}
}