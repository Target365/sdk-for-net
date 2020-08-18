namespace Target365.Sdk
{
	/// <summary>
	/// Strex user validity values.
	/// </summary>
	public enum StrexUserValidity
	{
		/// <summary>
		/// Not registered (0)
		/// </summary>
		Unregistered = 0,

		/// <summary>
		/// Registered, but not valid for licensed purchase (1)
		/// </summary>
		Partial = 1,

		/// <summary>
		/// Registered and valid for licensed purchase (2)
		/// </summary>
		Full = 2,

		/// <summary>
		/// Barred (3)
		/// </summary>
		Barred = 3
	}
}