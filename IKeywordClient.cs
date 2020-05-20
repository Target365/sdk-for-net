using System.Threading;
using System.Threading.Tasks;

namespace Target365.Sdk
{	
	/// <summary>
	/// Keyword client interface.
	/// </summary>
	public interface IKeywordClient
	{
		/// <summary>
		/// Gets keywords.
		/// </summary>
		/// <param name="shortNumberId">Filter for short number id (exact string match).</param>
		/// <param name="keywordText">Filter for keyword text (contains match).</param>
		/// <param name="mode">Filter for mode (exact string match).</param>
		/// <param name="tag">Filter for tag (exact string match).</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		Task<Keyword[]> GetAllKeywordsAsync(string shortNumberId = null, string keywordText = null, string mode = null, string tag = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Posts a text message.
		/// </summary>
		/// <param name="keyword">Keyword object.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		Task<string> CreateKeywordAsync(Keyword keyword, CancellationToken cancellationToken = default);

		/// <summary>
		/// Gets a keyword.
		/// </summary>
		/// <param name="keywordId">Keyword id.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		Task<Keyword> GetKeywordAsync(string keywordId, CancellationToken cancellationToken = default);

		/// <summary>
		/// Updates a keyword.
		/// </summary>
		/// <param name="keyword">Updated keyword.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		Task UpdateKeywordAsync(Keyword keyword, CancellationToken cancellationToken = default);

		/// <summary>
		/// Deletes a keyword.
		/// </summary>
		/// <param name="keywordId">Keyword id.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		Task DeleteKeywordAsync(string keywordId, CancellationToken cancellationToken = default);
	}
}