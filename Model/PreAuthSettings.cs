namespace Target365.Sdk
{
	/// <summary>
	/// Preauth settings
	/// </summary>
	public class PreAuthSettings
	{
		/// <summary>
		/// Info message sent before preauth message
		/// </summary>
		public string InfoText { get; set; }

		/// <summary>
		/// Sender of info message
		/// </summary>
		public string InfoSender { get; set; }

		/// <summary>
		/// Text inserted before preauth text
		/// </summary>
		public string PrefixMessage { get; set; }

		/// <summary>
		/// Text inserted after preauth text
		/// </summary>
		public string PostfixMessage { get; set; }

		/// <summary>
		/// Delay in minutes between info message and preauth message
		/// </summary>
		public decimal? Delay { get; set; }

		/// <summary>
		/// MerchantId to perform preauth on
		/// </summary>
		public string MerchantId { get; set; }

		/// <summary>
		/// Service description for Strex "Min Side"
		/// </summary>
		public string ServiceDescription { get; set; }
		
		/// <summary>
		/// If settings are active
		/// </summary>
		public bool? Active { get; set; }
	}
}
