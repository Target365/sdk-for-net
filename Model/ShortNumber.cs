namespace Target365.Sdk
{
	/// <summary>
	/// Short number.
	/// </summary>
	public class ShortNumber
	{
		/// <summary>
		/// Short number id.
		/// </summary>
		public string ShortNumberId { get; set; }

		/// <summary>
		/// Tow-letter country code. 'NO' for Norwegian.
		/// </summary>
		public string CountryCode { get; set; }

		/// <summary>
		/// Short number.
		/// </summary>
		public string Number { get; set; }

		/// <summary>
		/// Three-letter currency code. 'NOK' for Norwegian Krone.
		/// </summary>
		public string CurrencyCode { get; set; }
	}
}
