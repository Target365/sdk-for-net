using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Target365.Sdk
{
	/// <summary>
	/// Verification client interface.
	/// </summary>
	public interface IVerificationClient
	{
		/// <summary>
		/// Verifies an incoming http request from Target365 like an in-message or delivery report forward.
		/// </summary>
		/// <param name="request">HttpRequestMessage object.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		Task VerifyRequestSignatureAsync(HttpRequestMessage request, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Verifies an incoming http request from Target365 like an in-message or delivery report forward.
		/// </summary>
		/// <param name="method">HTTP method verb.</param>
		/// <param name="uri">Full request uri.</param>
		/// <param name="contentBytes">Raw http content.</param>
		/// <param name="signatureString">Signature string from HTTP header 'X-ECDSA-Signature'.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		Task VerifyRequestSignatureAsync(string method, string uri, byte[] contentBytes, string signatureString, CancellationToken cancellationToken = default(CancellationToken));
	}
}
