using System;
using System.Runtime.Serialization;
using System.Security.Cryptography;

namespace Target365.Sdk
{
	/// <summary>
	/// Public key.
	/// </summary>
	[DataContract(Namespace = "http://schemas.target365.io/core")]
	public class PublicKey
	{
		/// <summary>
		/// Account id.
		/// </summary>
		[DataMember]
		public long AccountId { get; set; }

		/// <summary>
		/// Key name.
		/// </summary>
		[DataMember]
		public string Name { get; set; }

		/// <summary>
		/// Public key in DER(ANS.1) base64 format.
		/// </summary>
		[DataMember]
		public string PublicKeyString { get; set; }

		/// <summary>
		/// Signature algorithm.
		/// </summary>
		[DataMember]
		public string SignAlgo { get; set; }

		/// <summary>
		/// Hash algorithm.
		/// </summary>
		[DataMember]
		public string HashAlgo { get; set; }

		/// <summary>
		/// Created time.
		/// </summary>
		[DataMember]
		public DateTimeOffset Created { get; set; }

		/// <summary>
		/// Last modified time.
		/// </summary>
		[DataMember]
		public DateTimeOffset LastModified { get; set; }

		/// <summary>
		/// Not-usable-before time.
		/// </summary>
		[DataMember]
		public DateTimeOffset NotUsableBefore { get; set; }

		/// <summary>
		/// Expiry time.
		/// </summary>
		[DataMember]
		public DateTimeOffset Expiry { get; set; }
	}
}