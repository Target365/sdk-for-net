using System.Threading;
using System.Threading.Tasks;

namespace Target365.Sdk
{	
	/// <summary>
	/// Strex client interface.
	/// </summary>
	public interface IPublicKeysClient
	{
		/// <summary>
		/// Gets server public key used for signing outgoing http requests like delivery reports and in-messages.
		/// </summary>
		/// <param name="keyName">Key name.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		Task<PublicKey> GetServerPublicKeyAsync(string keyName, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Gets all client public keys.
		/// </summary>
		/// <param name="cancellationToken">Cancellation token.</param>
		Task<PublicKey[]> GetClientPublicKeysAsync(CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Gets a client public key.
		/// </summary>
		/// <param name="keyName">Key name.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		Task<PublicKey> GetClientPublicKeyAsync(string keyName, CancellationToken cancellationToken = default(CancellationToken));
	}
}