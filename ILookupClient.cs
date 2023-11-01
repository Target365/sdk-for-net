using System.Threading;
using System.Threading.Tasks;

namespace Target365.Sdk
{
	/// <summary>
	/// Lookup client interface.
	/// </summary>
	public interface ILookupClient
	{
		/// <summary>
		/// Looks up a mobile phone number.
		/// </summary>
		/// <param name="msisdn">Mobile phone number. Format is full international msisdn including a leading plus, ex: '+4798079008'.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		Task<Lookup> LookupAsync(string msisdn, CancellationToken cancellationToken = default);

		/// <summary>
		/// Looks up address info from free text (name, address...).
		/// </summary>
		/// <param name="freeText">Free text like name or address.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		Task<Lookup[]> LookupFreeTextAsync(string freeText, CancellationToken cancellationToken = default);
	}
}