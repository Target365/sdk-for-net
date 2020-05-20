using System.Threading;
using System.Threading.Tasks;

namespace Target365.Sdk
{
	/// <summary>
	/// In-message client interface.
	/// </summary>
	public interface IInMessageClient
	{
		/// <summary>
		/// Gets an in-message.
		/// </summary>
		/// <param name="shortNumberId">Short number id.</param>
		/// <param name="transactionId">Transaction id.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		Task<InMessage> GetInMessageAsync(string shortNumberId, string transactionId, CancellationToken cancellationToken = default);
	}
}