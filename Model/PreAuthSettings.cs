using System.Runtime.Serialization;

namespace Target365.Sdk
{
	/// <summary>
	/// Preauth settings
	/// </summary>
	[DataContract(Namespace = "http://schemas.target365.io/core")]
	public class PreAuthSettings
	{
		/// <summary>
		/// Info message sent before preauth message
		/// </summary>
		[DataMember]
		public string InfoText { get; set; }

		/// <summary>
		/// Sender of info message
		/// </summary>
		[DataMember]
		public string InfoSender { get; set; }

		/// <summary>
		/// Text inserted before preauth text
		/// </summary>
		[DataMember]
		public string PrefixMessage { get; set; }

		/// <summary>
		/// Text inserted after preauth text
		/// </summary>
		[DataMember]
		public string PostfixMessage { get; set; }

		/// <summary>
		/// Delay in minutes between info message and preauth message
		/// </summary>
		[DataMember]
		public decimal? Delay { get; set; }

		/// <summary>
		/// MerchantId to perform preauth on
		/// </summary>
		[DataMember]
		public string MerchantId { get; set; }

		/// <summary>
		/// Service description for Strex "Min Side"
		/// </summary>
		[DataMember]
		public string ServiceDescription { get; set; }

		/// <summary>
		/// If settings are active
		/// </summary>
		[DataMember]
		public bool? Active { get; set; }
	}
}
