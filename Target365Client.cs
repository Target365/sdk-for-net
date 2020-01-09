using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json.Converters;

namespace Target365.Sdk
{
	/// <summary>
	/// Target365 client.
	/// </summary>
	public class Target365Client : ILookupClient, IKeywordClient, IInMessageClient, IOutMessageClient, IStrexClient, IPublicKeysClient, IVerificationClient, IDisposable
	{
		private static Lazy<HttpClient> _staticHttpClient = new Lazy<HttpClient>(CreateHttpClient, true);
		private static Dictionary<string, PublicKey> _publicKeys = new Dictionary<string, PublicKey>();
		private HttpClient _httpClient = new HttpClient();
		private readonly string _keyName;
		private readonly CngKey _cngPublicKey;

		private static JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
		{
			Converters = new List<JsonConverter>
			{
				new StringEnumConverter { CamelCaseText = false }
			},
			ContractResolver = new DefaultContractResolver
			{
				NamingStrategy = new CamelCaseNamingStrategy { ProcessDictionaryKeys = false }
			}
		};

		/// <summary>
		/// Minimum HTTP timeout.
		/// </summary>
		public static TimeSpan MinimumHttpTimeout = TimeSpan.FromSeconds(30);

		/// <summary>
		/// ServiceClient constructor.
		/// </summary>
		/// <param name="baseUrl">Base url - provided by Target365.</param>
		/// <param name="keyName">Key name registered as a public key with Target365.</param>
		/// <param name="privateKey">Private key as a base64-encoded string.</param>
		/// <param name="httpTimeout">Http timeout for client. Minimum 30 seconds and default value is 60.</param>
		public Target365Client(Uri baseUrl, string keyName, string privateKey, TimeSpan? httpTimeout = null)
		{
			if (baseUrl == null) throw new ArgumentException("baseUrl cannot be null.");
			if (string.IsNullOrEmpty(keyName)) throw new ArgumentException($"{nameof(keyName)} cannot be null or empty.");
			if (string.IsNullOrEmpty(privateKey)) throw new ArgumentException($"{nameof(privateKey)} cannot be null or empty.");

#if (!DEBUG)
			if (baseUrl.Scheme != "https") throw new ArgumentException($"{nameof(baseUrl)} must have https scheme.");
#endif

			if (httpTimeout == null)
				httpTimeout = TimeSpan.FromSeconds(60);

			if (httpTimeout < MinimumHttpTimeout)
				throw new ArgumentException($"httpTimeout {httpTimeout} too low. Minimum is {MinimumHttpTimeout}");

			_keyName = keyName;
			if (privateKey.Length > 300)
				_cngPublicKey = CngKey.Import(Convert.FromBase64String(privateKey), CngKeyBlobFormat.GenericPrivateBlob);
			else
				_cngPublicKey = CngKey.Import(Convert.FromBase64String(privateKey), CngKeyBlobFormat.EccPrivateBlob);

			_httpClient.BaseAddress = baseUrl;
			_httpClient.DefaultRequestHeaders.Accept.Clear();
			_httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			_httpClient.Timeout = httpTimeout.Value;
		}

		/// <summary>
		/// Casts this service client as ILookupService.
		/// </summary>
		public ILookupClient AsLookupService() { return this as ILookupClient; }

		/// <summary>
		/// Casts this service client as IOutMessageService.
		/// </summary>
		public IOutMessageClient AsOutMessageService() { return this as IOutMessageClient; }

		/// <summary>
		/// Casts this service client as IKeywordService.
		/// </summary>
		public IKeywordClient AsKeywordService() { return this as IKeywordClient; }

		/// <summary>
		/// Casts this service client as IStrexService.
		/// </summary>
		public IStrexClient AsStrexService() { return this as IStrexClient; }

		/// <summary>
		/// Casts this service client as IPublicKeysClient.
		/// </summary>
		/// <returns></returns>
		public IPublicKeysClient AsPublicKeysClient() { return this as IPublicKeysClient; }

		/// <summary>
		/// Casts this service client as IRequestVerifier.
		/// </summary>
		public IVerificationClient AsRequestVerifier() { return this as IVerificationClient; }
		
		/// <summary>
		/// Pings the service and returns a hello message.
		/// </summary>
		/// <param name="baseUrl">Base url - provided by Target365.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		public static async Task<string> PingAsync(Uri baseUrl, CancellationToken cancellationToken = default(CancellationToken))
		{
			var request = new HttpRequestMessage(HttpMethod.Get, new Uri(baseUrl, "api/ping"));

			using (var response = await _staticHttpClient.Value.SendAsync(request, cancellationToken).ConfigureAwait(false))
			{
				if (!response.IsSuccessStatusCode)
					await ThrowExceptionFromResponseAsync(request, response).ConfigureAwait(false);

				return JsonConvert.DeserializeObject<string>(await response.Content.ReadAsStringAsync().ConfigureAwait(false), _jsonSerializerSettings);
			}
		}

		/// <summary>
		/// Pings the service and returns a hello message.
		/// </summary>
		/// <param name="cancellationToken">Cancellation token.</param>
		public async Task<string> PingAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			var request = new HttpRequestMessage(HttpMethod.Get, new Uri(_httpClient.BaseAddress, "api/ping"));
			
			using (var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false))
			{
				if (!response.IsSuccessStatusCode)
					await ThrowExceptionFromResponseAsync(request, response).ConfigureAwait(false);

				return JsonConvert.DeserializeObject<string>(await response.Content.ReadAsStringAsync().ConfigureAwait(false), _jsonSerializerSettings);
			}
		}

		/// <summary>
		/// Looks up address info on a mobile phone number.
		/// </summary>
		/// <param name="msisdn">Mobile phone number.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		public async Task<Lookup> LookupAsync(string msisdn, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (string.IsNullOrEmpty(msisdn)) throw new ArgumentException("msisdn cannot be null or empty string.");

			var request = new HttpRequestMessage(HttpMethod.Get, new Uri(_httpClient.BaseAddress, $"api/lookup?msisdn={WebUtility.UrlEncode(msisdn)}"));
			await SignRequest(request).ConfigureAwait(false);

			using (var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false))
			{
				if (response.StatusCode == HttpStatusCode.NotFound)
					return null;

				if (!response.IsSuccessStatusCode)
					await ThrowExceptionFromResponseAsync(request, response).ConfigureAwait(false);

				return JsonConvert.DeserializeObject<Lookup>(await response.Content.ReadAsStringAsync().ConfigureAwait(false), _jsonSerializerSettings);
			}
		}

		/// <summary>
		/// Creates a new keyword.
		/// </summary>
		/// <param name="keyword">Keyword object.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		public async Task<string> CreateKeywordAsync(Keyword keyword, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (keyword == null) throw new ArgumentNullException(nameof(keyword));

			var content = new StringContent(JsonConvert.SerializeObject(keyword, _jsonSerializerSettings), Encoding.UTF8, "application/json");
			var request = new HttpRequestMessage(HttpMethod.Post, new Uri(_httpClient.BaseAddress, "api/keywords"))
			{
				Content = content
			};

			await SignRequest(request).ConfigureAwait(false);

			using (var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false))
			{
				if (!response.IsSuccessStatusCode)
					await ThrowExceptionFromResponseAsync(request, response).ConfigureAwait(false);

				return response.Headers.Location.AbsoluteUri.Split('/').Last();
			}
		}

		/// <summary>
		/// Gets all keywords.
		/// </summary>
		/// <param name="shortNumberId">Filter for short number id (exact string match).</param>
		/// <param name="keywordText">Filter for keyword text (contains match).</param>
		/// <param name="mode">Filter for mode (exact string match).</param>
		/// <param name="tag">Filter for tag (exact string match).</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		public async Task<Keyword[]> GetAllKeywordsAsync(string shortNumberId = null, string keywordText = null, string mode = null, string tag = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			var uri = "api/keywords";
			var queryParams = new Dictionary<string, string>
			{
				{ "shortNumberId", shortNumberId },
				{ "keywordText", keywordText },
				{ "mode", mode },
				{ "tag", tag }
			}.Where(x => x.Value != null).ToArray();

			if (queryParams.Any())
				uri = string.Join("?", uri, string.Join("&", queryParams.Select(x => $"{x.Key}={WebUtility.UrlEncode(x.Value)}")));

			var request = new HttpRequestMessage(HttpMethod.Get, new Uri(_httpClient.BaseAddress, uri));
			await SignRequest(request).ConfigureAwait(false);

			using (var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false))
			{
				if (!response.IsSuccessStatusCode)
					await ThrowExceptionFromResponseAsync(request, response).ConfigureAwait(false);

				return JsonConvert.DeserializeObject<Keyword[]>(await response.Content.ReadAsStringAsync().ConfigureAwait(false), _jsonSerializerSettings);
			}
		}

		/// <summary>
		/// Gets a keyword.
		/// </summary>
		/// <param name="keywordId">Keyword id.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		public async Task<Keyword> GetKeywordAsync(string keywordId, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (string.IsNullOrEmpty(keywordId)) throw new ArgumentException("keywordId cannot be null or empty string.");

			var request = new HttpRequestMessage(HttpMethod.Get, new Uri(_httpClient.BaseAddress, $"api/keywords/{keywordId}"));
			await SignRequest(request).ConfigureAwait(false);

			using (var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false))
			{
				if (response.StatusCode == HttpStatusCode.NotFound)
					return null;

				if (!response.IsSuccessStatusCode)
					await ThrowExceptionFromResponseAsync(request, response).ConfigureAwait(false);

				return JsonConvert.DeserializeObject<Keyword>(await response.Content.ReadAsStringAsync().ConfigureAwait(false), _jsonSerializerSettings);
			}
		}

		/// <summary>
		/// Updates a keyword.
		/// </summary>
		/// <param name="keyword">Updated keyword.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		public async Task UpdateKeywordAsync(Keyword keyword, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (keyword == null) throw new ArgumentNullException(nameof(keyword));
			if (keyword.KeywordId == null) throw new ArgumentNullException(nameof(keyword.KeywordId));

			var content = new StringContent(JsonConvert.SerializeObject(keyword, _jsonSerializerSettings), Encoding.UTF8, "application/json");
			var request = new HttpRequestMessage(HttpMethod.Put, new Uri(_httpClient.BaseAddress, $"api/keywords/{keyword.KeywordId}"))
			{
				Content = content
			};

			await SignRequest(request).ConfigureAwait(false);

			using (var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false))
			{
				if (!response.IsSuccessStatusCode)
					await ThrowExceptionFromResponseAsync(request, response).ConfigureAwait(false);
			}
		}

		/// <summary>
		/// Deletes a keyword.
		/// </summary>
		/// <param name="keywordId">Keyword id.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		public async Task DeleteKeywordAsync(string keywordId, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (string.IsNullOrEmpty(keywordId)) throw new ArgumentException($"{nameof(keywordId)} cannot be null or empty string.");

			var request = new HttpRequestMessage(HttpMethod.Delete, new Uri(_httpClient.BaseAddress, $"api/keywords/{keywordId}"));
			await SignRequest(request).ConfigureAwait(false);

			using (var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false))
			{
				if (!response.IsSuccessStatusCode)
					await ThrowExceptionFromResponseAsync(request, response).ConfigureAwait(false);
			}
		}

		/// <summary>
		/// Gets an in-message.
		/// </summary>
		/// <param name="shortNumberId">Short number id.</param>
		/// <param name="transactionId">In-message transaction id.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		public async Task<InMessage> GetInMessageAsync(string shortNumberId, string transactionId, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrEmpty(shortNumberId)) throw new ArgumentException("shortNumberId cannot be null or empty string.");
			if (string.IsNullOrEmpty(transactionId)) throw new ArgumentException("transactionId cannot be null or empty string.");

			var requestUri = new Uri(_httpClient.BaseAddress, $"api/in-messages/{shortNumberId}/{WebUtility.UrlEncode(transactionId)}");
			var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
			await SignRequest(request).ConfigureAwait(false);

			using (var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false))
			{
				if (response.StatusCode == HttpStatusCode.NotFound)
					return null;

				if (!response.IsSuccessStatusCode)
					await ThrowExceptionFromResponseAsync(request, response).ConfigureAwait(false);

				return JsonConvert.DeserializeObject<InMessage>(await response.Content.ReadAsStringAsync().ConfigureAwait(false), _jsonSerializerSettings);
			}
		}

		/// <summary>
		/// Creates a new out-message.
		/// </summary>
		/// <param name="message">Out-message object.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		public async Task<string> CreateOutMessageAsync(OutMessage message, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (message == null) throw new ArgumentNullException(nameof(message));

			var content = new StringContent(JsonConvert.SerializeObject(message, _jsonSerializerSettings), Encoding.UTF8, "application/json");
			var request = new HttpRequestMessage(HttpMethod.Post, new Uri(_httpClient.BaseAddress, "api/out-messages"))
			{
				Content = content
			};

			await SignRequest(request).ConfigureAwait(false);

			using (var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false))
			{
				if (!response.IsSuccessStatusCode)
					await ThrowExceptionFromResponseAsync(request, response).ConfigureAwait(false);

				return response.Headers.Location.AbsoluteUri.Split('/').Last();
			}
		}

		/// <summary>
		/// Creates a new out-message batch.
		/// </summary>
		/// <param name="messages">Out-messages array.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		public async Task CreateOutMessageBatchAsync(OutMessage[] messages, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (messages == null) throw new ArgumentNullException(nameof(messages));

			var content = new StringContent(JsonConvert.SerializeObject(messages, _jsonSerializerSettings), Encoding.UTF8, "application/json");
			var request = new HttpRequestMessage(HttpMethod.Post, new Uri(_httpClient.BaseAddress, "api/out-messages/batch"))
			{
				Content = content
			};

			await SignRequest(request).ConfigureAwait(false);

			using (var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false))
			{
				if (!response.IsSuccessStatusCode)
					await ThrowExceptionFromResponseAsync(request, response).ConfigureAwait(false);
			}
		}

		/// <summary>
		/// Gets an out-message.
		/// </summary>
		/// <param name="transactionId">Transaction id.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		public async Task<OutMessage> GetOutMessageAsync(string transactionId, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (string.IsNullOrEmpty(transactionId)) throw new ArgumentException("transactionId cannot be null or empty string.");

			var request = new HttpRequestMessage(HttpMethod.Get, new Uri(_httpClient.BaseAddress, $"api/out-messages/{WebUtility.UrlEncode(transactionId)}"));
			await SignRequest(request).ConfigureAwait(false);

			using (var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false))
			{
				if (response.StatusCode == HttpStatusCode.NotFound)
					return null;

				if (!response.IsSuccessStatusCode)
					await ThrowExceptionFromResponseAsync(request, response).ConfigureAwait(false);

				return JsonConvert.DeserializeObject<OutMessage>(await response.Content.ReadAsStringAsync().ConfigureAwait(false), _jsonSerializerSettings);
			}
		}

		/// <summary>
		/// Updates a future scheduled out-message.
		/// </summary>
		/// <param name="message">Updated message.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		public async Task UpdateOutMessageAsync(OutMessage message, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (message == null) throw new ArgumentNullException(nameof(message));
			if (message.TransactionId == null) throw new ArgumentNullException(nameof(message.TransactionId));

			var content = new StringContent(JsonConvert.SerializeObject(message, _jsonSerializerSettings), Encoding.UTF8, "application/json");
			var request = new HttpRequestMessage(HttpMethod.Put, new Uri(_httpClient.BaseAddress, $"api/out-messages/{WebUtility.UrlEncode(message.TransactionId)}"))
			{
				Content = content
			};

			await SignRequest(request).ConfigureAwait(false);

			using (var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false))
			{
				if (!response.IsSuccessStatusCode)
					await ThrowExceptionFromResponseAsync(request, response).ConfigureAwait(false);
			}
		}

		/// <summary>
		/// Deletes a future sheduled out-message.
		/// </summary>
		/// <param name="transactionId">Transaction id.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		public async Task DeleteOutMessageAsync(string transactionId, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (string.IsNullOrEmpty(transactionId)) throw new ArgumentException("transactionId cannot be null or empty string.");

			var request = new HttpRequestMessage(HttpMethod.Delete, new Uri(_httpClient.BaseAddress, $"api/out-messages/{WebUtility.UrlEncode(transactionId)}"));
			await SignRequest(request).ConfigureAwait(false);

			using (var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false))
			{
				if (!response.IsSuccessStatusCode)
					await ThrowExceptionFromResponseAsync(request, response).ConfigureAwait(false);
			}
		}

		/// <summary>
		/// Prepare MSISDNs for later sendings. This can greatly improves sending performance.
		/// </summary>
		/// <param name="msisdns">Msisdns to prepare.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		public async Task PrepareMsisdnsAsync(string[] msisdns, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (msisdns == null || msisdns.Length == 0) throw new ArgumentException(nameof(msisdns) + " cannot be null or empty.");

			var content = new StringContent(JsonConvert.SerializeObject(msisdns, _jsonSerializerSettings), Encoding.UTF8, "application/json");
			var request = new HttpRequestMessage(HttpMethod.Post, new Uri(_httpClient.BaseAddress, "api/prepare-msisdns"))
			{
				Content = content
			};

			await SignRequest(request).ConfigureAwait(false);

			using (var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false))
			{
				if (!response.IsSuccessStatusCode)
					await ThrowExceptionFromResponseAsync(request, response).ConfigureAwait(false);
			}
		}

		/// <summary>
		/// Gets all merchant ids.
		/// </summary>
		/// <param name="cancellationToken">Cancellation token.</param>
		public async Task<StrexMerchant[]> GetMerchantIdsAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			var request = new HttpRequestMessage(HttpMethod.Get, new Uri(_httpClient.BaseAddress, "api/strex/merchants"));
			await SignRequest(request).ConfigureAwait(false);

			using (var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false))
			{
				if (!response.IsSuccessStatusCode)
					await ThrowExceptionFromResponseAsync(request, response).ConfigureAwait(false);

				return JsonConvert.DeserializeObject<StrexMerchant[]>(await response.Content.ReadAsStringAsync().ConfigureAwait(false), _jsonSerializerSettings);
			}
		}

		/// <summary>
		/// Gets a merchant.
		/// </summary>
		/// <param name="merchantId">merchant id.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		public async Task<StrexMerchant> GetMerchantAsync(string merchantId, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (string.IsNullOrEmpty(merchantId)) throw new ArgumentException($"{nameof(merchantId)} cannot be null or empty string.");

			var request = new HttpRequestMessage(HttpMethod.Get, new Uri(_httpClient.BaseAddress, $"api/strex/merchants/{WebUtility.UrlEncode(merchantId)}"));
			await SignRequest(request).ConfigureAwait(false);

			using (var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false))
			{
				if (response.StatusCode == HttpStatusCode.NotFound)
					return null;

				if (!response.IsSuccessStatusCode)
					await ThrowExceptionFromResponseAsync(request, response).ConfigureAwait(false);

				return JsonConvert.DeserializeObject<StrexMerchant>(await response.Content.ReadAsStringAsync().ConfigureAwait(false), _jsonSerializerSettings);
			}
		}

		/// <summary>
		/// Creates/updates a merchant.
		/// </summary>
		/// <param name="merchant">merchant object.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		public async Task SaveMerchantAsync(StrexMerchant merchant, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (merchant == null) throw new ArgumentNullException(nameof(merchant));
			if (merchant.MerchantId == null) throw new ArgumentNullException(nameof(merchant.MerchantId));

			var content = new StringContent(JsonConvert.SerializeObject(merchant, _jsonSerializerSettings), Encoding.UTF8, "application/json");
			var request = new HttpRequestMessage(HttpMethod.Put, new Uri(_httpClient.BaseAddress, $"api/strex/merchants/{WebUtility.UrlEncode(merchant.MerchantId)}"))
			{
				Content = content
			};

			await SignRequest(request).ConfigureAwait(false);

			using (var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false))
			{
				if (!response.IsSuccessStatusCode)
					await ThrowExceptionFromResponseAsync(request, response).ConfigureAwait(false);
			}
		}

		/// <summary>
		/// Deletes a merchant.
		/// </summary>
		/// <param name="merchantId">Merchant id.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		public async Task DeleteMerchantAsync(string merchantId, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (string.IsNullOrEmpty(merchantId)) throw new ArgumentException($"{nameof(merchantId)} cannot be null or empty string.");

			var request = new HttpRequestMessage(HttpMethod.Delete, new Uri(_httpClient.BaseAddress, $"api/strex/merchants/{WebUtility.UrlEncode(merchantId)}"));
			await SignRequest(request).ConfigureAwait(false);

			using (var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false))
			{
				if (!response.IsSuccessStatusCode)
					await ThrowExceptionFromResponseAsync(request, response).ConfigureAwait(false);
			}
		}

		/// <summary>
		/// Creates/updates a merchant.
		/// </summary>
		/// <param name="oneTimePassword">One-time password object.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		public async Task CreateOneTimePasswordAsync(OneTimePassword oneTimePassword, CancellationToken cancellationToken = default)
		{
			if (oneTimePassword == null) throw new ArgumentException($"{nameof(oneTimePassword)} cannot be null.");

			var content = new StringContent(JsonConvert.SerializeObject(oneTimePassword, _jsonSerializerSettings), Encoding.UTF8, "application/json");
			var request = new HttpRequestMessage(HttpMethod.Post, new Uri(_httpClient.BaseAddress, "api/strex/one-time-passwords"))
			{
				Content = content
			};

			await SignRequest(request).ConfigureAwait(false);

			using (var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false))
			{
				if (!response.IsSuccessStatusCode)
					await ThrowExceptionFromResponseAsync(request, response).ConfigureAwait(false);
			}
		}

		/// <summary>
		/// Gets a one-time password.
		/// </summary>
		/// <param name="transactionId">Strex transaction id.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		public async Task<OneTimePassword> GetOneTimePasswordAsync(string transactionId, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (string.IsNullOrEmpty(transactionId)) throw new ArgumentException("transactionId cannot be null or empty string.");

			var request = new HttpRequestMessage(HttpMethod.Get, new Uri(_httpClient.BaseAddress, $"api/strex/one-time-passwords/{transactionId}"));
			await SignRequest(request).ConfigureAwait(false);

			using (var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false))
			{
				if (response.StatusCode == HttpStatusCode.NotFound)
					return null;

				if (!response.IsSuccessStatusCode)
					await ThrowExceptionFromResponseAsync(request, response).ConfigureAwait(false);

				return JsonConvert.DeserializeObject<OneTimePassword>(await response.Content.ReadAsStringAsync().ConfigureAwait(false), _jsonSerializerSettings);
			}
		}

		/// <summary>
		/// Creates a new strex transaction.
		/// </summary>
		/// <param name="transaction">Strex transaction object.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		public async Task<string> CreateStrexTransactionAsync(StrexTransaction transaction, CancellationToken cancellationToken = default)
		{
			if (transaction == null) throw new ArgumentException($"{nameof(transaction)} cannot be null.");

			var content = new StringContent(JsonConvert.SerializeObject(transaction, _jsonSerializerSettings), Encoding.UTF8, "application/json");
			var request = new HttpRequestMessage(HttpMethod.Post, new Uri(_httpClient.BaseAddress, "api/strex/transactions"))
			{
				Content = content
			};

			await SignRequest(request).ConfigureAwait(false);

			using (var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false))
			{
				if (!response.IsSuccessStatusCode)
					await ThrowExceptionFromResponseAsync(request, response).ConfigureAwait(false);

				return response.Headers.Location.AbsoluteUri.Split('/').Last();
			}
		}

		/// <summary>
		/// Gets a strex transaction.
		/// </summary>
		/// <param name="transactionId">Strex transaction id.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		public async Task<StrexTransaction> GetStrexTransactionAsync(string transactionId, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (string.IsNullOrEmpty(transactionId)) throw new ArgumentException("transactionId cannot be null or empty string.");

			var request = new HttpRequestMessage(HttpMethod.Get, new Uri(_httpClient.BaseAddress, $"api/strex/transactions/{transactionId}"));
			await SignRequest(request).ConfigureAwait(false);

			using (var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false))
			{
				if (response.StatusCode == HttpStatusCode.NotFound)
					return null;

				if (!response.IsSuccessStatusCode)
					await ThrowExceptionFromResponseAsync(request, response).ConfigureAwait(false);

				return JsonConvert.DeserializeObject<StrexTransaction>(await response.Content.ReadAsStringAsync().ConfigureAwait(false), _jsonSerializerSettings);
			}
		}

		/// <summary>
		/// Reverses a strex transaction and returns the resulting reversal transaction id.
		/// </summary>
		/// <param name="transactionId">Strex transaction id.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		public async Task<string> ReverseStrexTransactionAsync(string transactionId, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (transactionId == null) throw new ArgumentException($"{nameof(transactionId)} cannot be null.");

			var requestUrl = new Uri(_httpClient.BaseAddress, $"api/strex/transactions/{WebUtility.UrlEncode(transactionId)}");
			var request = new HttpRequestMessage(HttpMethod.Delete, requestUrl);

			await SignRequest(request).ConfigureAwait(false);

			using (var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false))
			{
				if (!response.IsSuccessStatusCode)
					await ThrowExceptionFromResponseAsync(request, response).ConfigureAwait(false);

				return response.Headers.Location.AbsoluteUri.Split('/').Last();
			}
		}

		/// <summary>
		/// Gets server public key used for signing outgoing http requests like delivery reports and in-messages.
		/// </summary>
		/// <param name="keyName">Key name.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		public async Task<PublicKey> GetServerPublicKeyAsync(string keyName, CancellationToken cancellationToken = default(CancellationToken))
		{
			var request = new HttpRequestMessage(HttpMethod.Get, new Uri(_httpClient.BaseAddress, $"api/server/public-keys/{WebUtility.UrlEncode(keyName)}"));
			await SignRequest(request);

			using (var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false))
			{
				if (response.StatusCode == HttpStatusCode.NotFound)
					return null;

				if (!response.IsSuccessStatusCode)
					await ThrowExceptionFromResponseAsync(request, response).ConfigureAwait(false);

				return JsonConvert.DeserializeObject<PublicKey>(await response.Content.ReadAsStringAsync().ConfigureAwait(false), _jsonSerializerSettings);
			}
		}

		/// <summary>
		/// Gets client public key used for verifying incoming http requests.
		/// </summary>
		/// <param name="cancellationToken">Cancellation token.</param>
		public async Task<PublicKey[]> GetClientPublicKeysAsync(CancellationToken cancellationToken = default)
		{
			var request = new HttpRequestMessage(HttpMethod.Get, new Uri(_httpClient.BaseAddress, "api/client/public-keys"));
			await SignRequest(request);

			using (var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false))
			{
				if (response.StatusCode == HttpStatusCode.NotFound)
					return null;

				if (!response.IsSuccessStatusCode)
					await ThrowExceptionFromResponseAsync(request, response).ConfigureAwait(false);

				return JsonConvert.DeserializeObject<PublicKey[]>(await response.Content.ReadAsStringAsync().ConfigureAwait(false), _jsonSerializerSettings);
			}
		}

		/// <summary>
		/// Gets a client public key.
		/// </summary>
		/// <param name="keyName">Key name.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		public async Task<PublicKey> GetClientPublicKeyAsync(string keyName, CancellationToken cancellationToken = default)
		{
			var request = new HttpRequestMessage(HttpMethod.Get, new Uri(_httpClient.BaseAddress, $"api/client/public-keys/{WebUtility.UrlEncode(keyName)}"));
			await SignRequest(request);

			using (var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false))
			{
				if (response.StatusCode == HttpStatusCode.NotFound)
					return null;

				if (!response.IsSuccessStatusCode)
					await ThrowExceptionFromResponseAsync(request, response).ConfigureAwait(false);

				return JsonConvert.DeserializeObject<PublicKey>(await response.Content.ReadAsStringAsync().ConfigureAwait(false), _jsonSerializerSettings);
			}
		}

		/// <summary>
		/// Verifies the signature of incoming requests from Target365, like forwarded delivery reports or in-messages.
		/// Throws an UnauthorizedAccessException exception if the request couldn't be verified.
		/// </summary>
		/// <param name="request">HttpRequestMessage object.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <exception cref="UnauthorizedAccessException">Thrown if the request couldn't be verified.</exception>
		/// <exception cref="ArgumentNullException">Public key couldn't be found.</exception>
		public async Task VerifyRequestSignatureAsync(HttpRequestMessage request, CancellationToken cancellationToken = default(CancellationToken))
		{
			var method = request.Method.ToString().ToLowerInvariant();
			var uri = request.RequestUri.AbsoluteUri.ToLowerInvariant();
			var contentBytes = request.Content != null ? await request.Content.ReadAsByteArrayAsync() : null;
			var signature = request.Headers.GetValues("X-ECDSA-Signature").FirstOrDefault();
			await VerifyRequestSignatureAsync(method, uri, contentBytes, signature, cancellationToken).ConfigureAwait(false);
		}

		/// <summary>
		/// Verifies the signature of incoming requests from Target365, like forwarded delivery reports or in-messages.
		/// Throws an UnauthorizedAccessException exception if the request couldn't be verified.
		/// </summary>
		/// <param name="method">Request method.</param>
		/// <param name="uri">Request uri.</param>
		/// <param name="contentBytes">Request raw content, in bytes.</param>
		/// <param name="signatureString">Signature string, from HTTP header 'X-ECDSA-Signature'.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <exception cref="UnauthorizedAccessException">Thrown if the request couldn't be verified.</exception>
		/// <exception cref="ArgumentNullException">Public key couldn't be found.</exception>
		public async Task VerifyRequestSignatureAsync(string method, string uri, byte[] contentBytes, string signatureString, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (signatureString == null)
				throw new UnauthorizedAccessException("No signature parameter.");

			var parts = signatureString.Split(':');

			if (parts.Length != 4)
				throw new UnauthorizedAccessException("Signature parameter had invalid format.");

			method = method.ToLowerInvariant();
			uri = uri.ToLowerInvariant();
			var keyName = parts[0];
			var timestamp = long.Parse(parts[1]);
			var nounce = parts[2];
			var signature = parts[3];

			var timestampNow = Convert.ToInt64((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds);

			if (Math.Abs(timestampNow - timestamp) > (5 * 60))
				throw new UnauthorizedAccessException("Clock-drift too big.");

			var contentHash = "";

			if (contentBytes != null && contentBytes.Length > 0)
			{
				using (var sha256 = SHA256.Create())
				{
					contentHash = Convert.ToBase64String(sha256.ComputeHash(contentBytes));
				}
			}

			if (!_publicKeys.TryGetValue(keyName, out var publicKey))
			{
				publicKey = await GetServerPublicKeyAsync(keyName, cancellationToken).ConfigureAwait(false);
				_publicKeys[keyName] = publicKey ?? throw new ArgumentNullException(nameof(keyName), $"public key '{keyName}' not found.");
			}

			var message = $"{method}{uri}{timestamp}{nounce}{contentHash}";
			var signatureBytes = Convert.FromBase64String(signature);
			var cngEcPublicBlob = DerAns1ToCngEcPublicBlob(Convert.FromBase64String(publicKey.PublicKeyString));

			using (var ecdsa = new ECDsaCng(CngKey.Import(cngEcPublicBlob, CngKeyBlobFormat.EccPublicBlob)))
			{
#if NET46
				if (!ecdsa.VerifyData(Encoding.UTF8.GetBytes(message), signatureBytes))
					throw new UnauthorizedAccessException("Incorrect signature parameter in request.");
#else
				if (!ecdsa.VerifyData(Encoding.UTF8.GetBytes(message), signatureBytes, HashAlgorithmName.SHA256))
					throw new UnauthorizedAccessException("Incorrect signature parameter in request.");
#endif
			}
		}

		/// <summary>
		/// Disposes all resources associated with this instance.
		/// </summary>
		public void Dispose()
		{
			_httpClient.Dispose();
		}

		private static HttpClient CreateHttpClient()
		{
			var httpClient = new HttpClient();
			httpClient.DefaultRequestHeaders.Accept.Clear();
			httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			httpClient.Timeout = TimeSpan.FromSeconds(60);
			return httpClient;
		}

		private byte[] DerAns1ToCngEcPublicBlob(byte[] DerAns1Bytes)
		{
			var secp256r1Prefix = Convert.FromBase64String("MFkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDQgAE");
			var cngBlobPrefix = Convert.FromBase64String("RUNTMSAAAAA=");
			var cngBlob = new byte[cngBlobPrefix.Length + DerAns1Bytes.Length - secp256r1Prefix.Length];
			Buffer.BlockCopy(cngBlobPrefix, 0, cngBlob, 0, cngBlobPrefix.Length);
			Buffer.BlockCopy(DerAns1Bytes, secp256r1Prefix.Length, cngBlob, cngBlobPrefix.Length, DerAns1Bytes.Length - secp256r1Prefix.Length);
			return cngBlob;
		}

		private async Task SignRequest(HttpRequestMessage request)
		{
			var method = request.Method.ToString().ToLowerInvariant();
			var uri = request.RequestUri.AbsoluteUri.ToLowerInvariant();
			var keyName = _keyName;
			var timestamp = Convert.ToInt64((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds);
			var nonce = Guid.NewGuid().ToString();
			var contentHash = "";

			if (request.Content != null)
			{
				using (var sha256 = SHA256.Create())
				{
					var contentBytes = await request.Content.ReadAsByteArrayAsync().ConfigureAwait(false);

					if (contentBytes?.Length > 0)
						contentHash = Convert.ToBase64String(sha256.ComputeHash(contentBytes));
				}
			}

			var message = $"{method}{uri}{timestamp}{nonce}{contentHash}";

			if (_cngPublicKey.KeySize >= 1024)
			{
				using (var rsa = new RSACng(_cngPublicKey))
				{
					var signatureString = Convert.ToBase64String(rsa.SignData(Encoding.UTF8.GetBytes(message), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1));
					request.Headers.Authorization = new AuthenticationHeaderValue("HMAC", $"{keyName}:{timestamp}:{nonce}:{signatureString}");
				}
			}
			else
			{
				using (var ecdsa = new ECDsaCng(_cngPublicKey))
				{
#if NET46
					var signatureString = Convert.ToBase64String(ecdsa.SignData(Encoding.UTF8.GetBytes(message)));
#else
					var signatureString = Convert.ToBase64String(ecdsa.SignData(Encoding.UTF8.GetBytes(message), HashAlgorithmName.SHA256));
#endif
					request.Headers.Authorization = new AuthenticationHeaderValue("HMAC", $"{keyName}:{timestamp}:{nonce}:{signatureString}");
				}
			}
		}

		private class ErrorHolder
		{
			public string Message { get; set; }
		}

		private static async Task ThrowExceptionFromResponseAsync(HttpRequestMessage request, HttpResponseMessage response)
		{
			string message = null;

			try
			{
				var httpError = JsonConvert.DeserializeObject<ErrorHolder>(await response.Content.ReadAsStringAsync().ConfigureAwait(false), _jsonSerializerSettings);
				message = httpError?.Message;
			}
			catch
			{
			}

			throw new ServiceClientException(message, request, response);
		}
	}
}
