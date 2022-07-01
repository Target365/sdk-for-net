using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Target365.Sdk
{
	/// <summary>
	/// Keyword.
	/// </summary>
	[DataContract(Namespace = "http://schemas.target365.io/core")]
	public class Keyword
	{
		/// <summary>
		/// Keyword id returned by Target365.
		/// </summary>
		[DataMember]
		public string KeywordId { get; set; }

		/// <summary>
		/// Short number associated with keyword.
		/// </summary>
		[DataMember(IsRequired = true)]
		public string ShortNumberId { get; set; }

		/// <summary>
		/// Keyword text.
		/// </summary>
		[DataMember(IsRequired = true)]
		public string KeywordText { get; set; }

		/// <summary>
		/// Keyword mode. Can be 'Text', 'Wildcard' or 'Regex'.
		/// </summary>
		[DataMember(IsRequired = true)]
		public string Mode { get; set; }

		/// <summary>
		/// Keyword forward url to post incoming messages.
		/// </summary>
		[DataMember(IsRequired = true)]
		public string ForwardUrl { get; set; }

		/// <summary>
		/// Preauth settings
		/// </summary>
		public PreAuthSettings PreAuthSettings { get; set; }

		/// <summary>
		/// Whether keyword is enabled.
		/// </summary>
		[DataMember(IsRequired = true)]
		public bool Enabled { get; set; }

		/// <summary>
		/// Creation date.
		/// </summary>
		[DataMember]
		public DateTimeOffset Created { get; set; }

		/// <summary>
		/// Last modified date.
		/// </summary>
		[DataMember]
		public DateTimeOffset LastModified { get; set; }

		/// <summary>
		/// Custom properties associated with keyword. Will be propagated to incoming messages.
		/// </summary>
		[DataMember]
		public Dictionary<string, object> CustomProperties { get; set; } = new Dictionary<string, object>();

		/// <summary>
		/// Tags associated with keyword. Can be used for statistics and grouping.
		/// </summary>
		[DataMember]
		public ICollection<string> Tags { get; set; } = new HashSet<string>();

		/// <summary>
		/// Alias keywords associated with keyword.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public ICollection<string> Aliases { get; set; } = new HashSet<string>();
	}
}
