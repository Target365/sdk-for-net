using System.Threading;
using System.Threading.Tasks;

namespace Target365.Sdk
{	
	/// <summary>
	/// Strex client interface.
	/// </summary>
	public interface IStrexClient
	{
		/// <summary>
		/// Gets all merchants.
		/// </summary>
		/// <param name="cancellationToken">Cancellation token.</param>
		Task<StrexMerchant[]> GetMerchantIdsAsync(CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Gets a merchant.
		/// </summary>
		/// <param name="merchantId">merchant id.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		Task<StrexMerchant> GetMerchantAsync(string merchantId, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Creates/updates a merchant.
		/// </summary>
		/// <param name="merchant">merchant object.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		Task SaveMerchantAsync(StrexMerchant merchant, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Deletes a merchant.
		/// </summary>
		/// <param name="merchantId">merchant id.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		Task DeleteMerchantAsync(string merchantId, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Creates a new one-time password.
		/// </summary>
		/// <param name="oneTimePassword">One-time password object.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		Task CreateOneTimePasswordAsync(OneTimePassword oneTimePassword, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Gets a one-time password.
		/// </summary>
		/// <param name="transactionId">Strex transaction id.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		Task<OneTimePassword> GetOneTimePasswordAsync(string transactionId, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Creates a new strex transaction.
		/// </summary>
		/// <param name="transactionId">Strex transaction object.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		Task<string> CreateStrexTransactionAsync(StrexTransaction transaction, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Gets a strex transaction.
		/// </summary>
		/// <param name="transactionId">Strex transaction id.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		Task<StrexTransaction> GetStrexTransactionAsync(string transactionId, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		/// Reverses a strex transaction and returns the resulting reversal transaction id.
		/// </summary>
		/// <param name="transaction">Strex transaction id.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		Task<string> ReverseStrexTransactionAsync(string transactionId, CancellationToken cancellationToken = default(CancellationToken));
	}
}