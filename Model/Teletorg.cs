using System;
using System.Runtime.Serialization;

namespace Target365.Sdk
{
	/// <summary>
	/// Strex teletorg data.
	/// </summary>
	[DataContract(Namespace = "http://schemas.target365.io/core")]
	public class Teletorg
	{
		/// <summary>
		/// The called teletorg number.
		/// </summary>
		[DataMember]
		public string BNumber { get; set; }

		/// <summary>
		/// Start time of the charged teletorg voice call.
		/// </summary>
		[DataMember]
		public DateTimeOffset StartTime { get; set; }

		/// <summary>
		/// Charged voice call duration, in seconds.
		/// </summary>
		[DataMember]
		public int Duration { get; set; }

		/// <summary>
		/// Start price for the charged voice call. Start price should be in 1/100 NOK (e.g. 1 NOK = 100 Øre)
		/// </summary>
		[DataMember]
		public decimal StartPrice { get; set; }
	}
}
