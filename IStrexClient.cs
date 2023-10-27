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
		Task<StrexMerchant[]> GetMerchantIdsAsync(CancellationToken cancellationToken = default);

		/// <summary>
		/// Gets a merchant.
		/// </summary>
		/// <param name="merchantId">merchant id.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		Task<StrexMerchant> GetMerchantAsync(string merchantId, CancellationToken cancellationToken = default);

		/// <summary>
		/// Creates/updates a merchant.
		/// </summary>
		/// <param name="merchant">merchant object.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		Task SaveMerchantAsync(StrexMerchant merchant, CancellationToken cancellationToken = default);

		/// <summary>
		/// Deletes a merchant.
		/// </summary>
		/// <param name="merchantId">merchant id.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		Task DeleteMerchantAsync(string merchantId, CancellationToken cancellationToken = default);

		/// <summary>
		/// Creates a new one-time password.
		/// </summary>
		/// <param name="oneTimePassword">One-time password object.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		Task CreateOneTimePasswordAsync(OneTimePassword oneTimePassword, CancellationToken cancellationToken = default);

		/// <summary>
		/// Gets a one-time password.
		/// </summary>
		/// <param name="transactionId">Strex transaction id.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		Task<OneTimePassword> GetOneTimePasswordAsync(string transactionId, CancellationToken cancellationToken = default);

		/// <summary>
		/// Creates a new strex transaction.
		/// </summary>
		/// <param name="transaction">Strex transaction object.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		Task<string> CreateStrexTransactionAsync(StrexTransaction transaction, CancellationToken cancellationToken = default);

		/// <summary>
		/// Gets a strex transaction.
		/// </summary>
		/// <param name="transactionId">Strex transaction id.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		Task<StrexTransaction> GetStrexTransactionAsync(string transactionId, CancellationToken cancellationToken = default);

		/// <summary>
		/// Reverses a strex transaction and returns the resulting reversal transaction id.
		/// </summary>
		/// <param name="transactionId">Strex transaction id.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		Task<string> ReverseStrexTransactionAsync(string transactionId, CancellationToken cancellationToken = default);

		/// <summary>
		/// Gets Strex user info.
		/// </summary>
		/// <param name="recipient">Recipient msisdn.</param>
		/// <param name="merchantId">MerchantId (optional).</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		Task<StrexUserInfo> GetStrexUserInfoAsync(string recipient, string merchantId, CancellationToken cancellationToken = default);

		/// <summary>
		/// Gets Strex user info V2.
		/// </summary>
		/// <param name="recipient">Recipient msisdn.</param>
		/// <param name="merchantId">MerchantId (optional).</param>
		/// <param name="serviceCode">Service code.</param>
		/// <param name="price">Price in kr.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		Task<StrexUserInfoV2> GetStrexUserInfoV2Async(string recipient, string merchantId, string serviceCode, decimal price, CancellationToken cancellationToken = default);

		/// <summary>
		/// Gets Strex user info V3.
		/// </summary>
		/// <param name="recipient">Recipient msisdn.</param>
		/// <param name="merchantId">MerchantId (optional).</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		Task<StrexUserInfoV3> GetStrexUserInfoV3Async(string recipient, string merchantId, CancellationToken cancellationToken = default);

		/// <summary>
		/// Gets a one-click config.
		/// </summary>
		/// <param name="configId">Config id.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		Task<OneClickConfig> GetOneClickConfigAsync(string configId, CancellationToken cancellationToken = default);

		/// <summary>
		/// Creates/updates a one-click config.
		/// </summary>
		/// <param name="config">One-click config object.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		Task SaveOneClickConfigAsync(OneClickConfig config, CancellationToken cancellationToken = default);

		/// <summary>
		/// Initiates Strex-registation by SMS.
		/// </summary>
		/// <param name="registrationSms">Strex registration sms.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		Task SendStrexRegistrationSmsAsync(StrexRegistrationSms registrationSms, CancellationToken cancellationToken = default);
	}
}