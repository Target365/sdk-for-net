using System.Runtime.Serialization;

namespace Target365.Sdk
{
	/// <summary>
	/// Strex user validity values.
	/// </summary>
	[DataContract(Namespace = "http://schemas.target365.io/core")]
	public class StrexUserInfoV2
	{
		/// <summary>
		/// Result 0 = OK
		/// </summary>
		[DataMember]
		public string Result { get; set; }

		/// <summary>
		/// Description of errors
		/// </summary>
		[DataMember]
		public string ResultDescription { get; set; }
	}
}