using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Target365.Sdk
{
	/// <summary>
	/// Out-message client interface.
	/// </summary>
	public interface IOutMessageClient
	{
		/// <summary>
		/// Creates a new out-message.
		/// </summary>
		/// <param name="message">Out-message object.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		Task<string> CreateOutMessageAsync(OutMessage message, CancellationToken cancellationToken = default);

		/// <summary>
		/// Creates a new out-message batch.
		/// </summary>
		/// <param name="messages">Out-messages array.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		Task CreateOutMessageBatchAsync(OutMessage[] messages, CancellationToken cancellationToken = default);

		/// <summary>
		/// Gets a message.
		/// </summary>
		/// <param name="transactionId">Transaction id.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		Task<OutMessage> GetOutMessageAsync(string transactionId, CancellationToken cancellationToken = default);

		/// <summary>
		/// Updates a future scheduled message.
		/// </summary>
		/// <param name="message">Updated message.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		Task UpdateOutMessageAsync(OutMessage message, CancellationToken cancellationToken = default);

		/// <summary>
		/// Deletes a future sheduled message.
		/// </summary>
		/// <param name="transactionId">Transaction id.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		Task DeleteOutMessageAsync(string transactionId, CancellationToken cancellationToken = default);

		/// <summary>
		/// Prepares msisdns for later sendings. This can greatly improves sending performance.
		/// </summary>
		/// <param name="msisdns">Msisdns to prepare.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		Task PrepareMsisdnsAsync(string[] msisdns, CancellationToken cancellationToken = default);

		/// <summary>
		/// Get out-message export stream in CSV format.
		/// </summary>
		/// <param name="from">From time.</param>
		/// <param name="to">To time.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		Task<Stream> GetOutMessageExportAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default);
	}
}