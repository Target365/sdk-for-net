namespace Target365.Sdk
{
	/// <summary>
	/// Strex PreAuth
	/// </summary>
	public class StrexPreAuth
	{
		/// <summary>
		/// Msisdn
		/// </summary>
		public string Msisdn { get; set; }

		/// <summary>
		/// Short number
		/// </summary>
		public string ShortNumber { get; set; }

		/// <summary>
		/// Merchant Id
		/// </summary>
		public string MerchantId { get; set; }

		/// <summary>
		/// Service Id. Deprecated.
		/// </summary>
		public string ServiceId { get; set; }

		/// <summary>
		/// Service description for Strex "Min Side"
		/// </summary>
		public string ServiceDescription { get; set; }

		/// <summary>
		/// Text inserted before preauth text
		/// </summary>
		public string PrefixMessage { get; set; }

		/// <summary>
		/// Text inserted after preauth text
		/// </summary>
		public string PostfixMessage { get; set; }

		/// <summary>
		/// If preauth should be without confirmation message to end user.
		/// </summary>
		public bool IsSilentPreAuthorization { get; set; }

		/// <summary>
		/// Age
		/// </summary>
		public int Age { get; set; }
	}
}
