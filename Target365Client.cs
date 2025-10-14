using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Target365.Sdk
{
	/// <summary>
	/// Target365 client.
	/// </summary>
	public class Target365Client : ILookupClient, IKeywordClient, IInMessageClient, IOutMessageClient, IStrexClient, IPublicKeysClient, IVerificationClient, IDisposable
	{
		private static readonly Lazy<HttpClient> _staticHttpClient = new(CreateHttpClient, true);
		private static readonly Dictionary<string, PublicKey> _publicKeys = new();
		private readonly HttpClient _httpClient;
		private readonly string _keyName;
		private readonly byte[] _cngPrivateKeyBytes;
		private readonly JsonSerializerOptions _jsonSerializerOptions;

		/// <summary>
		/// Minimum HTTP timeout.
		/// </summary>
		public static TimeSpan MinimumHttpTimeout = TimeSpan.FromSeconds(10);

		/// <summary>
		/// Initializes a new Target365Client.
		/// </summary>
		/// <param name="baseUrl">Base url - provided by Target365.</param>
		/// <param name="keyName">Key name registered as a public key with Target365.</param>
		/// <param name="privateKey">Private key as a base64-encoded string.</param>
		/// <param name="httpTimeout">Http timeout for client. Minimum is 10 seconds and default value is 30 seconds.</param>
		/// <param name="httpMessageHandler">Http message handler to inject.</param>
		public Target365Client(Uri baseUrl, string keyName, string privateKey, TimeSpan? httpTimeout = null, HttpMessageHandler httpMessageHandler = null)
		{
			if (baseUrl == null) throw new ArgumentException("baseUrl cannot be null.");
			if (string.IsNullOrEmpty(keyName)) throw new ArgumentException($"{nameof(keyName)} cannot be null or empty.");
			if (string.IsNullOrEmpty(privateKey)) throw new ArgumentException($"{nameof(privateKey)} cannot be null or empty.");

#if (!DEBUG)
			if (baseUrl.Scheme != "https") throw new ArgumentException($"{nameof(baseUrl)} must have https scheme.");
#endif

			if (httpTimeout == null)
				httpTimeout = TimeSpan.FromSeconds(30);

			if (httpTimeout < MinimumHttpTimeout)
				throw new ArgumentException($"httpTimeout {httpTimeout} too low. Minimum is {MinimumHttpTimeout}");

			_keyName = keyName;
			_cngPrivateKeyBytes = Convert.FromBase64String(privateKey);

			try
			{
				using var ecdsa = CryptoUtils.GetEcdsaFromPrivateKey(_cngPrivateKeyBytes);
			}
			catch
			{
				throw new ArgumentException($"{nameof(privateKey)} cannot be parsed properly.");
			}

			_httpClient = new HttpClient(httpMessageHandler ?? new HttpClientHandler());
			_httpClient.BaseAddress = baseUrl;
			_httpClient.DefaultRequestHeaders.Accept.Clear();
			_httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			_httpClient.DefaultRequestHeaders.Add("X-Sdk", ".NET");
			_httpClient.DefaultRequestHeaders.Add("X-Sdk-Version", "" + Assembly.GetExecutingAssembly().GetName().Version);
			_httpClient.Timeout = httpTimeout.Value;

			_jsonSerializerOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };
			_jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
			_jsonSerializerOptions.Converters.Add(new StringObjectDictionaryJsonConverter());
		}

		/// <summary>
		/// Casts this service client as ILookupService.
		/// </summary>
		public ILookupClient AsLookupService() { return this; }

		/// <summary>
		/// Casts this service client as IOutMessageService.
		/// </summary>
		public IOutMessageClient AsOutMessageService() { return this; }

		/// <summary>
		/// Casts this service client as IKeywordService.
		/// </summary>
		public IKeywordClient AsKeywordService() { return this; }

		/// <summary>
		/// Casts this service client as IStrexService.
		/// </summary>
		public IStrexClient AsStrexService() { return this; }

		/// <summary>
		/// Casts this service client as IPublicKeysClient.
		/// </summary>
		/// <returns></returns>
		public IPublicKeysClient AsPublicKeysClient() { return this; }

		/// <summary>
		/// Casts this service client as IRequestVerifier.
		/// </summary>
		public IVerificationClient AsRequestVerifier() { return this; }

		/// <summary>
		/// Pings the service and returns a hello message.
		/// </summary>
		/// <param name="baseUrl">Base url - provided by Target365.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		public static async Task<string> PingAsync(Uri baseUrl, CancellationToken cancellationToken = default)
		{
			using var request = new HttpRequestMessage(HttpMethod.Get, new Uri(baseUrl, "api/ping"));
			using var response = await _staticHttpClient.Value.SendAsync(request, cancellationToken).ConfigureAwait(false);

			if (!response.IsSuccessStatusCode)
				await ThrowExceptionFromResponseAsync(request, response).ConfigureAwait(false);

			return JsonSerializer.Deserialize<string>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
		}

		/// <summary>
		/// Pings the service and returns a hello message.
		/// </summary>
		/// <param name="cancellationToken">Cancellation token.</param>
		public async Task<string> PingAsync(CancellationToken cancellationToken = default)
		{
			using var request = new HttpRequestMessage(HttpMethod.Get, new Uri(_httpClient.BaseAddress, "api/ping"));
			using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

			if (!response.IsSuccessStatusCode)
				await ThrowExceptionFromResponseAsync(request, response).ConfigureAwait(false);

			return Deserialize<string>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
		}

		/// <summary>
		/// Looks up address info on a mobile phone number.
		/// </summary>
		/// <param name="msisdn">Mobile phone number.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		public async Task<Lookup> LookupAsync(string msisdn, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrEmpty(msisdn)) throw new ArgumentException("msisdn cannot be null or empty string.");

			using var request = new HttpRequestMessage(HttpMethod.Get, new Uri(_httpClient.BaseAddress, $"api/lookup?msisdn={WebUtility.UrlEncode(msisdn)}"));
			await SignRequest(request).ConfigureAwait(false);
			using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

			if (response.StatusCode == HttpStatusCode.NotFound)
				return null;

			if (!response.IsSuccessStatusCode)
				await ThrowExceptionFromResponseAsync(request, response).ConfigureAwait(false);

			return Deserialize<Lookup>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
		}

		/// <summary>
		/// Looks up address info from free text (name, address...).
		/// </summary>
		/// <param name="freeText">Free text like name or address.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		public async Task<Lookup[]> LookupFreeTextAsync(string freeText, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrEmpty(freeText)) throw new ArgumentException("freeText cannot be null or empty string.");

			using var request = new HttpRequestMessage(HttpMethod.Get, new Uri(_httpClient.BaseAddress, $"api/lookup/freetext?input={WebUtility.UrlEncode(freeText)}"));
			await SignRequest(request).ConfigureAwait(false);
			using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

			if (response.StatusCode == HttpStatusCode.NotFound)
				return null;

			if (!response.IsSuccessStatusCode)
				await ThrowExceptionFromResponseAsync(request, response).ConfigureAwait(false);

			return Deserialize<Lookup[]>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
		}

		/// <summary>
		/// Creates a new keyword.
		/// </summary>
		/// <param name="keyword">Keyword object.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		public async Task<string> CreateKeywordAsync(Keyword keyword, CancellationToken cancellationToken = default)
		{
			if (keyword == null) throw new ArgumentNullException(nameof(keyword));

			var content = new StringContent(Serialize(keyword), Encoding.UTF8, "application/json");
			using var request = new HttpRequestMessage(HttpMethod.Post, new Uri(_httpClient.BaseAddress, "api/keywords"))
			{
				Content = content
			};

			await SignRequest(request).ConfigureAwait(false);
			using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

			if (!response.IsSuccessStatusCode)
				await ThrowExceptionFromResponseAsync(request, response).ConfigureAwait(false);

			return response.Headers.Location.AbsoluteUri.Split('/').Last();
		}

		/// <summary>
		/// Gets all keywords.
		/// </summary>
		/// <param name="shortNumberId">Filter for short number id (exact string match).</param>
		/// <param name="keywordText">Filter for keyword text (contains match).</param>
		/// <param name="mode">Filter for mode (exact string match).</param>
		/// <param name="tag">Filter for tag (exact string match).</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		public async Task<Keyword[]> GetAllKeywordsAsync(string shortNumberId = null, string keywordText = null, string mode = null, string tag = null, CancellationToken cancellationToken = default)
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

			using var request = new HttpRequestMessage(HttpMethod.Get, new Uri(_httpClient.BaseAddress, uri));
			await SignRequest(request).ConfigureAwait(false);
			using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

			if (!response.IsSuccessStatusCode)
				await ThrowExceptionFromResponseAsync(request, response).ConfigureAwait(false);

			return Deserialize<Keyword[]>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
		}

		/// <summary>
		/// Gets a keyword.
		/// </summary>
		/// <param name="keywordId">Keyword id.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		public async Task<Keyword> GetKeywordAsync(string keywordId, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrEmpty(keywordId)) throw new ArgumentException("keywordId cannot be null or empty string.");

			using var request = new HttpRequestMessage(HttpMethod.Get, new Uri(_httpClient.BaseAddress, $"api/keywords/{keywordId}"));
			await SignRequest(request).ConfigureAwait(false);
			using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

			if (response.StatusCode == HttpStatusCode.NotFound)
				return null;

			if (!response.IsSuccessStatusCode)
				await ThrowExceptionFromResponseAsync(request, response).ConfigureAwait(false);

			return Deserialize<Keyword>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
		}

		/// <summary>
		/// Updates a keyword.
		/// </summary>
		/// <param name="keyword">Updated keyword.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		public async Task UpdateKeywordAsync(Keyword keyword, CancellationToken cancellationToken = default)
		{
			if (keyword == null) throw new ArgumentNullException(nameof(keyword));
			if (keyword.KeywordId == null) throw new ArgumentNullException(nameof(keyword.KeywordId));

			var content = new StringContent(Serialize(keyword), Encoding.UTF8, "application/json");
			using var request = new HttpRequestMessage(HttpMethod.Put, new Uri(_httpClient.BaseAddress, $"api/keywords/{keyword.KeywordId}"))
			{
				Content = content
			};

			await SignRequest(request).ConfigureAwait(false);
			using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

			if (!response.IsSuccessStatusCode)
				await ThrowExceptionFromResponseAsync(request, response).ConfigureAwait(false);
		}

		/// <summary>
		/// Deletes a keyword.
		/// </summary>
		/// <param name="keywordId">Keyword id.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		public async Task DeleteKeywordAsync(string keywordId, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrEmpty(keywordId)) throw new ArgumentException($"{nameof(keywordId)} cannot be null or empty string.");

			using var request = new HttpRequestMessage(HttpMethod.Delete, new Uri(_httpClient.BaseAddress, $"api/keywords/{keywordId}"));
			await SignRequest(request).ConfigureAwait(false);
			using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

			if (!response.IsSuccessStatusCode)
				await ThrowExceptionFromResponseAsync(request, response).ConfigureAwait(false);
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
			using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
			await SignRequest(request).ConfigureAwait(false);
			using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

			if (response.StatusCode == HttpStatusCode.NotFound)
				return null;

			if (!response.IsSuccessStatusCode)
				await ThrowExceptionFromResponseAsync(request, response).ConfigureAwait(false);

			return Deserialize<InMessage>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
		}

		/// <summary>
		/// Creates a new out-message.
		/// </summary>
		/// <param name="message">Out-message object.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		public async Task<string> CreateOutMessageAsync(OutMessage message, CancellationToken cancellationToken = default)
		{
			if (message == null) throw new ArgumentNullException(nameof(message));

			var content = new StringContent(Serialize(message), Encoding.UTF8, "application/json");
			using var request = new HttpRequestMessage(HttpMethod.Post, new Uri(_httpClient.BaseAddress, "api/out-messages"))
			{
				Content = content
			};

			await SignRequest(request).ConfigureAwait(false);
			using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

			if (!response.IsSuccessStatusCode)
				await ThrowExceptionFromResponseAsync(request, response).ConfigureAwait(false);

			return response.Headers.Location.AbsoluteUri.Split('/').Last();
		}

		/// <summary>
		/// Creates a new out-message batch.
		/// </summary>
		/// <param name="messages">Out-messages array.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		public async Task CreateOutMessageBatchAsync(OutMessage[] messages, CancellationToken cancellationToken = default)
		{
			if (messages == null) throw new ArgumentNullException(nameof(messages));

			var content = new StringContent(Serialize(messages), Encoding.UTF8, "application/json");
			using var request = new HttpRequestMessage(HttpMethod.Post, new Uri(_httpClient.BaseAddress, "api/out-messages/batch"))
			{
				Content = content
			};

			await SignRequest(request).ConfigureAwait(false);
			using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

			if (!response.IsSuccessStatusCode)
				await ThrowExceptionFromResponseAsync(request, response).ConfigureAwait(false);
		}

		/// <summary>
		/// Gets an out-message.
		/// </summary>
		/// <param name="transactionId">Transaction id.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		public async Task<OutMessage> GetOutMessageAsync(string transactionId, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrEmpty(transactionId)) throw new ArgumentException("transactionId cannot be null or empty string.");

			using var request = new HttpRequestMessage(HttpMethod.Get, new Uri(_httpClient.BaseAddress, $"api/out-messages/{WebUtility.UrlEncode(transactionId)}"));
			await SignRequest(request).ConfigureAwait(false);
			using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

			if (response.StatusCode == HttpStatusCode.NotFound)
				return null;

			if (!response.IsSuccessStatusCode)
				await ThrowExceptionFromResponseAsync(request, response).ConfigureAwait(false);

			return Deserialize<OutMessage>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
		}

		/// <summary>
		/// Updates a future scheduled out-message.
		/// </summary>
		/// <param name="message">Updated message.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		public async Task UpdateOutMessageAsync(OutMessage message, CancellationToken cancellationToken = default)
		{
			if (message == null) throw new ArgumentNullException(nameof(message));
			if (message.TransactionId == null) throw new ArgumentNullException(nameof(message.TransactionId));

			var content = new StringContent(Serialize(message), Encoding.UTF8, "application/json");
			using var request = new HttpRequestMessage(HttpMethod.Put, new Uri(_httpClient.BaseAddress, $"api/out-messages/{WebUtility.UrlEncode(message.TransactionId)}"))
			{
				Content = content
			};

			await SignRequest(request).ConfigureAwait(false);
			using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

			if (!response.IsSuccessStatusCode)
				await ThrowExceptionFromResponseAsync(request, response).ConfigureAwait(false);
		}

		/// <summary>
		/// Deletes a future sheduled out-message.
		/// </summary>
		/// <param name="transactionId">Transaction id.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		public async Task DeleteOutMessageAsync(string transactionId, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrEmpty(transactionId)) throw new ArgumentException("transactionId cannot be null or empty string.");

			using var request = new HttpRequestMessage(HttpMethod.Delete, new Uri(_httpClient.BaseAddress, $"api/out-messages/{WebUtility.UrlEncode(transactionId)}"));
			await SignRequest(request).ConfigureAwait(false);
			using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

			if (!response.IsSuccessStatusCode)
				await ThrowExceptionFromResponseAsync(request, response).ConfigureAwait(false);
		}

		/// <summary>
		/// Prepare MSISDNs for later sendings. This can greatly improves sending performance.
		/// </summary>
		/// <param name="msisdns">Msisdns to prepare.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		public async Task PrepareMsisdnsAsync(string[] msisdns, CancellationToken cancellationToken = default)
		{
			if (msisdns == null || msisdns.Length == 0) throw new ArgumentException(nameof(msisdns) + " cannot be null or empty.");

			var content = new StringContent(Serialize(msisdns), Encoding.UTF8, "application/json");
			using var request = new HttpRequestMessage(HttpMethod.Post, new Uri(_httpClient.BaseAddress, "api/prepare-msisdns"))
			{
				Content = content
			};

			await SignRequest(request).ConfigureAwait(false);
			using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

			if (!response.IsSuccessStatusCode)
				await ThrowExceptionFromResponseAsync(request, response).ConfigureAwait(false);
		}

		/// <summary>
		/// Get out-message export in CSV format.
		/// </summary>
		/// <param name="from">From time.</param>
		/// <param name="to">To time.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		public async Task<Stream> GetOutMessageExportAsync(DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken = default)
		{
			var requestUrl = $"api/export/out-messages?from={WebUtility.UrlEncode(from.ToString("o"))}&to={WebUtility.UrlEncode(to.ToString("o"))}";
			using var request = new HttpRequestMessage(HttpMethod.Get, new Uri(_httpClient.BaseAddress, requestUrl));
			await SignRequest(request).ConfigureAwait(false);
			var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

			if (!response.IsSuccessStatusCode)
				await ThrowExceptionFromResponseAsync(request, response).ConfigureAwait(false);

			return await response.Content.ReadAsStreamAsync();
		}

		/// <summary>
		/// Gets all merchant ids.
		/// </summary>
		/// <param name="cancellationToken">Cancellation token.</param>
		public async Task<StrexMerchant[]> GetMerchantIdsAsync(CancellationToken cancellationToken = default)
		{
			using var request = new HttpRequestMessage(HttpMethod.Get, new Uri(_httpClient.BaseAddress, "api/strex/merchants"));
			await SignRequest(request).ConfigureAwait(false);
			using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

			if (!response.IsSuccessStatusCode)
				await ThrowExceptionFromResponseAsync(request, response).ConfigureAwait(false);

			return Deserialize<StrexMerchant[]>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
		}

		/// <summary>
		/// Gets a one-click config.
		/// </summary>
		/// <param name="configId">Config id.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		public async Task<OneClickConfig> GetOneClickConfigAsync(string configId, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrEmpty(configId)) throw new ArgumentException($"{nameof(configId)} cannot be null or empty string.");

			using var request = new HttpRequestMessage(HttpMethod.Get, new Uri(_httpClient.BaseAddress, $"api/one-click/configs/{WebUtility.UrlEncode(configId)}"));
			await SignRequest(request).ConfigureAwait(false);
			using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

			if (response.StatusCode == HttpStatusCode.NotFound)
				return null;

			if (!response.IsSuccessStatusCode)
				await ThrowExceptionFromResponseAsync(request, response).ConfigureAwait(false);

			return Deserialize<OneClickConfig>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
		}

		/// <summary>
		/// Creates/updates a one-click config.
		/// </summary>
		/// <param name="config">one-click config object.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		public async Task SaveOneClickConfigAsync(OneClickConfig config, CancellationToken cancellationToken = default)
		{
			if (config == null) throw new ArgumentNullException(nameof(config));
			if (config.MerchantId == null) throw new ArgumentNullException(nameof(config.MerchantId));

			var content = new StringContent(Serialize(config), Encoding.UTF8, "application/json");
			using var request = new HttpRequestMessage(HttpMethod.Put, new Uri(_httpClient.BaseAddress, $"api/one-click/configs/{WebUtility.UrlEncode(config.ConfigId)}"))
			{
				Content = content
			};

			await SignRequest(request).ConfigureAwait(false);
			using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

			if (!response.IsSuccessStatusCode)
				await ThrowExceptionFromResponseAsync(request, response).ConfigureAwait(false);
		}

		/// <summary>
		/// Initiates Strex-registation by SMS.
		/// </summary>
		/// <param name="registrationSms">Strex registration sms.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		public async Task SendStrexRegistrationSmsAsync(StrexRegistrationSms registrationSms, CancellationToken cancellationToken = default)
		{
			if (registrationSms == null) throw new ArgumentNullException(nameof(registrationSms));
			if (registrationSms.MerchantId == null) throw new ArgumentNullException(nameof(registrationSms.MerchantId));
			if (registrationSms.Recipient == null) throw new ArgumentNullException(nameof(registrationSms.Recipient));
			if (registrationSms.TransactionId == null) throw new ArgumentNullException(nameof(registrationSms.TransactionId));

			var content = new StringContent(Serialize(registrationSms), Encoding.UTF8, "application/json");
			using var request = new HttpRequestMessage(HttpMethod.Post, new Uri(_httpClient.BaseAddress, "api/strex/registrationsms"))
			{
				Content = content
			};

			await SignRequest(request).ConfigureAwait(false);
			using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

			if (!response.IsSuccessStatusCode)
				await ThrowExceptionFromResponseAsync(request, response).ConfigureAwait(false);
		}

		/// <summary>
		/// Gets a merchant.
		/// </summary>
		/// <param name="merchantId">merchant id.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		public async Task<StrexMerchant> GetMerchantAsync(string merchantId, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrEmpty(merchantId)) throw new ArgumentException($"{nameof(merchantId)} cannot be null or empty string.");

			using var request = new HttpRequestMessage(HttpMethod.Get, new Uri(_httpClient.BaseAddress, $"api/strex/merchants/{WebUtility.UrlEncode(merchantId)}"));
			await SignRequest(request).ConfigureAwait(false);
			using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
			if (response.StatusCode == HttpStatusCode.NotFound)
				return null;

			if (!response.IsSuccessStatusCode)
				await ThrowExceptionFromResponseAsync(request, response).ConfigureAwait(false);

			return Deserialize<StrexMerchant>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
		}

		/// <summary>
		/// Creates/updates a merchant.
		/// </summary>
		/// <param name="merchant">merchant object.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		public async Task SaveMerchantAsync(StrexMerchant merchant, CancellationToken cancellationToken = default)
		{
			if (merchant == null) throw new ArgumentNullException(nameof(merchant));
			if (merchant.MerchantId == null) throw new ArgumentNullException(nameof(merchant.MerchantId));

			var content = new StringContent(Serialize(merchant), Encoding.UTF8, "application/json");
			using var request = new HttpRequestMessage(HttpMethod.Put, new Uri(_httpClient.BaseAddress, $"api/strex/merchants/{WebUtility.UrlEncode(merchant.MerchantId)}"))
			{
				Content = content
			};

			await SignRequest(request).ConfigureAwait(false);
			using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
			if (!response.IsSuccessStatusCode)
				await ThrowExceptionFromResponseAsync(request, response).ConfigureAwait(false);
		}

		/// <summary>
		/// Deletes a merchant.
		/// </summary>
		/// <param name="merchantId">Merchant id.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		public async Task DeleteMerchantAsync(string merchantId, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrEmpty(merchantId)) throw new ArgumentException($"{nameof(merchantId)} cannot be null or empty string.");

			using var request = new HttpRequestMessage(HttpMethod.Delete, new Uri(_httpClient.BaseAddress, $"api/strex/merchants/{WebUtility.UrlEncode(merchantId)}"));
			await SignRequest(request).ConfigureAwait(false);
			using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

			if (!response.IsSuccessStatusCode)
				await ThrowExceptionFromResponseAsync(request, response).ConfigureAwait(false);
		}

		/// <summary>
		/// Creates/updates a merchant.
		/// </summary>
		/// <param name="oneTimePassword">One-time password object.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		public async Task CreateOneTimePasswordAsync(OneTimePassword oneTimePassword, CancellationToken cancellationToken = default)
		{
			if (oneTimePassword == null) throw new ArgumentException($"{nameof(oneTimePassword)} cannot be null.");

			var content = new StringContent(Serialize(oneTimePassword), Encoding.UTF8, "application/json");
			using var request = new HttpRequestMessage(HttpMethod.Post, new Uri(_httpClient.BaseAddress, "api/strex/one-time-passwords"))
			{
				Content = content
			};

			await SignRequest(request).ConfigureAwait(false);
			using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

			if (!response.IsSuccessStatusCode)
				await ThrowExceptionFromResponseAsync(request, response).ConfigureAwait(false);
		}

		/// <summary>
		/// Gets a one-time password.
		/// </summary>
		/// <param name="transactionId">Strex transaction id.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		public async Task<OneTimePassword> GetOneTimePasswordAsync(string transactionId, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrEmpty(transactionId)) throw new ArgumentException("transactionId cannot be null or empty string.");

			using var request = new HttpRequestMessage(HttpMethod.Get, new Uri(_httpClient.BaseAddress, $"api/strex/one-time-passwords/{transactionId}"));
			await SignRequest(request).ConfigureAwait(false);
			using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

			if (response.StatusCode == HttpStatusCode.NotFound)
				return null;

			if (!response.IsSuccessStatusCode)
				await ThrowExceptionFromResponseAsync(request, response).ConfigureAwait(false);

			return Deserialize<OneTimePassword>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
		}

		/// <summary>
		/// Creates a new strex transaction.
		/// </summary>
		/// <param name="transaction">Strex transaction object.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		public async Task<string> CreateStrexTransactionAsync(StrexTransaction transaction, CancellationToken cancellationToken = default)
		{
			if (transaction == null) throw new ArgumentException($"{nameof(transaction)} cannot be null.");

			var content = new StringContent(Serialize(transaction), Encoding.UTF8, "application/json");
			using var request = new HttpRequestMessage(HttpMethod.Post, new Uri(_httpClient.BaseAddress, "api/strex/transactions"))
			{
				Content = content
			};

			await SignRequest(request).ConfigureAwait(false);
			using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

			if (!response.IsSuccessStatusCode)
				await ThrowExceptionFromResponseAsync(request, response).ConfigureAwait(false);

			return response.Headers.Location.AbsoluteUri.Split('/').Last();
		}

		/// <summary>
		/// Gets a strex transaction.
		/// </summary>
		/// <param name="transactionId">Strex transaction id.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		public async Task<StrexTransaction> GetStrexTransactionAsync(string transactionId, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrEmpty(transactionId)) throw new ArgumentException("transactionId cannot be null or empty string.");

			using var request = new HttpRequestMessage(HttpMethod.Get, new Uri(_httpClient.BaseAddress, $"api/strex/transactions/{transactionId}"));
			await SignRequest(request).ConfigureAwait(false);
			using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

			if (response.StatusCode == HttpStatusCode.NotFound)
				return null;

			if (!response.IsSuccessStatusCode)
				await ThrowExceptionFromResponseAsync(request, response).ConfigureAwait(false);

			return Deserialize<StrexTransaction>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
		}

		/// <summary>
		/// Reverses a strex transaction and returns the resulting reversal transaction id.
		/// </summary>
		/// <param name="transactionId">Strex transaction id.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		public async Task<string> ReverseStrexTransactionAsync(string transactionId, CancellationToken cancellationToken = default)
		{
			if (transactionId == null) throw new ArgumentException($"{nameof(transactionId)} cannot be null.");

			var requestUrl = new Uri(_httpClient.BaseAddress, $"api/strex/transactions/{WebUtility.UrlEncode(transactionId)}");
			using var request = new HttpRequestMessage(HttpMethod.Delete, requestUrl);
			await SignRequest(request).ConfigureAwait(false);
			using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

			if (!response.IsSuccessStatusCode)
				await ThrowExceptionFromResponseAsync(request, response).ConfigureAwait(false);

			return response.Headers.Location.AbsoluteUri.Split('/').Last();
		}

		/// <summary>
		/// Gets Strex user info.
		/// </summary>
		/// <param name="recipient">Recipient msisdn.</param>
		/// <param name="merchantId">MerchantId (optional).</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		public async Task<StrexUserInfo> GetStrexUserInfoAsync(string recipient, string merchantId, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrEmpty(recipient)) throw new ArgumentException("recipient cannot be null or empty string.");

			using var request = new HttpRequestMessage(HttpMethod.Get, new Uri(_httpClient.BaseAddress, $"api/strex/userinfo?recipient={WebUtility.UrlEncode(recipient)}"
				+ (string.IsNullOrEmpty(merchantId) ? "" : $"&merchantId={merchantId}")));

			await SignRequest(request).ConfigureAwait(false);
			using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

			if (!response.IsSuccessStatusCode)
				await ThrowExceptionFromResponseAsync(request, response).ConfigureAwait(false);

			return Deserialize<StrexUserInfo>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
		}

		/// <summary>
		/// Gets Strex user info V2.
		/// </summary>
		/// <param name="recipient">Recipient msisdn.</param>
		/// <param name="merchantId">MerchantId (optional).</param>
		/// <param name="serviceCode">Service code</param>
		/// <param name="price">Price in kr.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		public async Task<StrexUserInfoV2> GetStrexUserInfoV2Async(string recipient, string merchantId, string serviceCode, decimal price, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrEmpty(recipient)) throw new ArgumentException("recipient cannot be null or empty string.");
			if (string.IsNullOrEmpty(serviceCode)) throw new ArgumentException("serviceCode cannot be null or empty string.");
			if (price <= 0) throw new ArgumentException("price cannot be 0 or negative.");

			using var request = new HttpRequestMessage(HttpMethod.Get, new Uri(_httpClient.BaseAddress, $"api/strex/userinfo-v2?recipient={WebUtility.UrlEncode(recipient)}&serviceCode={serviceCode}&price={price}"
				+ (string.IsNullOrEmpty(merchantId) ? "" : $"&merchantId={merchantId}")));

			await SignRequest(request).ConfigureAwait(false);
			using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

			if (!response.IsSuccessStatusCode)
				await ThrowExceptionFromResponseAsync(request, response).ConfigureAwait(false);

			return Deserialize<StrexUserInfoV2>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
		}

		/// <summary>
		/// Gets Strex user info V3.
		/// </summary>
		/// <param name="recipient">Recipient msisdn.</param>
		/// <param name="merchantId">MerchantId (optional).</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		public async Task<StrexUserInfoV3> GetStrexUserInfoV3Async(string recipient, string merchantId, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrEmpty(recipient)) throw new ArgumentException("recipient cannot be null or empty string.");

			using var request = new HttpRequestMessage(HttpMethod.Get, new Uri(_httpClient.BaseAddress, $"api/strex/userinfo-v3?recipient={WebUtility.UrlEncode(recipient)}"
				+ (string.IsNullOrEmpty(merchantId) ? "" : $"&merchantId={merchantId}")));

			await SignRequest(request).ConfigureAwait(false);
			using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

			if (!response.IsSuccessStatusCode)
				await ThrowExceptionFromResponseAsync(request, response).ConfigureAwait(false);

			return Deserialize<StrexUserInfoV3>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
		}

		/// <summary>
		/// Gets a Strex preauthorization token.
		/// </summary>
		/// <param name="merchantId">Strex merchant id.</param>
		/// <param name="serviceId">Service id.</param>
		/// <param name="msisdn">Msisdn</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		public async Task<string> GetPreauthTokenAsync(string merchantId, string serviceId, string msisdn, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrEmpty(merchantId)) throw new ArgumentException($"{nameof(merchantId)} cannot be null or empty string.");
			if (string.IsNullOrEmpty(msisdn)) throw new ArgumentException($"{nameof(msisdn)} cannot be null or empty string.");

			using var request = new HttpRequestMessage(HttpMethod.Get, new Uri(_httpClient.BaseAddress, $"api/strex/token/{WebUtility.UrlEncode(merchantId)}?serviceId={WebUtility.UrlEncode(serviceId)}&msisdn={WebUtility.UrlEncode(msisdn)}"));
			await SignRequest(request).ConfigureAwait(false);
			using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

			if (response.StatusCode == HttpStatusCode.NotFound)
				return null;

			if (!response.IsSuccessStatusCode)
				await ThrowExceptionFromResponseAsync(request, response).ConfigureAwait(false);

			return await response.Content.ReadAsStringAsync();
		}

		/// <summary>
		/// Deletes a Strex preauthorization token.
		/// </summary>
		/// <param name="merchantId">Strex merchant id.</param>
		/// <param name="serviceId">Service id.</param>
		/// <param name="msisdn">Msisdn</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		public async Task DeletePreauthTokenAsync(string merchantId, string serviceId, string msisdn, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrEmpty(merchantId)) throw new ArgumentException($"{nameof(merchantId)} cannot be null or empty string.");
			if (string.IsNullOrEmpty(msisdn)) throw new ArgumentException($"{nameof(msisdn)} cannot be null or empty string.");

			using var request = new HttpRequestMessage(HttpMethod.Delete, new Uri(_httpClient.BaseAddress, $"api/strex/token/{WebUtility.UrlEncode(merchantId)}?serviceId={WebUtility.UrlEncode(serviceId)}&msisdn={WebUtility.UrlEncode(msisdn)}"));
			await SignRequest(request).ConfigureAwait(false);
			using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

			if (!response.IsSuccessStatusCode)
				await ThrowExceptionFromResponseAsync(request, response).ConfigureAwait(false);
		}

		/// <summary>
		/// Sends pin code to user for verification.
		/// </summary>
		/// <param name="pincode">Pin code object.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		public async Task SendPinCodeAsync(Pincode pincode, CancellationToken cancellationToken = default)
		{
			if (pincode == null) throw new ArgumentNullException(nameof(pincode));

			var content = new StringContent(Serialize(pincode), Encoding.UTF8, "application/json");
			using var request = new HttpRequestMessage(HttpMethod.Post, new Uri(_httpClient.BaseAddress, "api/pincodes"))
			{
				Content = content
			};

			await SignRequest(request).ConfigureAwait(false);
			using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

			if (!response.IsSuccessStatusCode)
				await ThrowExceptionFromResponseAsync(request, response).ConfigureAwait(false);
		}

		/// <summary>
		/// Verify pin code sent to user.
		/// </summary>
		/// <param name="transactionId">TransactionId used when creating pincode.</param>
		/// <param name="pincode">Pin code to check.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		public async Task<bool> VerifyPinCodeAsync(string transactionId, string pincode, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrEmpty(transactionId)) throw new ArgumentException("transactionId cannot be null or empty string.");
			if (string.IsNullOrEmpty(pincode)) throw new ArgumentException("pincode cannot be null or empty string.");

			using var request = new HttpRequestMessage(HttpMethod.Get, new Uri(_httpClient.BaseAddress, $"api/pincodes/verification"
				+ $"?transactionId={transactionId}&pincode={pincode}"));

			await SignRequest(request).ConfigureAwait(false);
			using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

			if (!response.IsSuccessStatusCode)
				await ThrowExceptionFromResponseAsync(request, response).ConfigureAwait(false);

			return Deserialize<bool>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
		}

		/// <summary>
		/// Gets server public key used for signing outgoing http requests like delivery reports and in-messages.
		/// </summary>
		/// <param name="keyName">Key name.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		public async Task<PublicKey> GetServerPublicKeyAsync(string keyName, CancellationToken cancellationToken = default)
		{
			using var request = new HttpRequestMessage(HttpMethod.Get, new Uri(_httpClient.BaseAddress, $"api/server/public-keys/{WebUtility.UrlEncode(keyName)}"));
			await SignRequest(request);
			using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

			if (response.StatusCode == HttpStatusCode.NotFound)
				return null;

			if (!response.IsSuccessStatusCode)
				await ThrowExceptionFromResponseAsync(request, response).ConfigureAwait(false);

			return Deserialize<PublicKey>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
		}

		/// <summary>
		/// Gets client public key used for verifying incoming http requests.
		/// </summary>
		/// <param name="cancellationToken">Cancellation token.</param>
		public async Task<PublicKey[]> GetClientPublicKeysAsync(CancellationToken cancellationToken = default)
		{
			using var request = new HttpRequestMessage(HttpMethod.Get, new Uri(_httpClient.BaseAddress, "api/client/public-keys"));
			await SignRequest(request);
			using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

			if (response.StatusCode == HttpStatusCode.NotFound)
				return null;

			if (!response.IsSuccessStatusCode)
				await ThrowExceptionFromResponseAsync(request, response).ConfigureAwait(false);

			return Deserialize<PublicKey[]>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
		}

		/// <summary>
		/// Gets a client public key.
		/// </summary>
		/// <param name="keyName">Key name.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		public async Task<PublicKey> GetClientPublicKeyAsync(string keyName, CancellationToken cancellationToken = default)
		{
			using var request = new HttpRequestMessage(HttpMethod.Get, new Uri(_httpClient.BaseAddress, $"api/client/public-keys/{WebUtility.UrlEncode(keyName)}"));
			await SignRequest(request);
			using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

			if (response.StatusCode == HttpStatusCode.NotFound)
				return null;

			if (!response.IsSuccessStatusCode)
				await ThrowExceptionFromResponseAsync(request, response).ConfigureAwait(false);

			return Deserialize<PublicKey>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
		}

		/// <summary>
		/// Verifies the signature of incoming requests from Target365, like forwarded delivery reports or in-messages.
		/// Throws an UnauthorizedAccessException exception if the request couldn't be verified.
		/// </summary>
		/// <param name="request">HttpRequestMessage object.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <exception cref="UnauthorizedAccessException">Thrown if the request couldn't be verified.</exception>
		/// <exception cref="ArgumentNullException">Public key couldn't be found.</exception>
		public async Task VerifyRequestSignatureAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
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
		public async Task VerifyRequestSignatureAsync(string method, string uri, byte[] contentBytes, string signatureString, CancellationToken cancellationToken = default)
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
				using var sha256 = SHA256.Create();
				contentHash = Convert.ToBase64String(sha256.ComputeHash(contentBytes));
			}

			PublicKey publicKey;

			lock (_publicKeys)
			{
				_publicKeys.TryGetValue(keyName, out publicKey);
			}

			if (publicKey == null)
			{
				publicKey = await GetServerPublicKeyAsync(keyName, cancellationToken).ConfigureAwait(false);

				lock (_publicKeys)
				{
					_publicKeys[keyName] = publicKey ?? throw new ArgumentNullException(nameof(keyName), $"public key '{keyName}' not found.");
				}
			}

			var message = $"{method}{uri}{timestamp}{nounce}{contentHash}";
			var signatureBytes = Convert.FromBase64String(signature);

			if (publicKey.SignAlgo == CryptoUtils.ECDsaP256)
			{
				using var ecdsa = CryptoUtils.GetEcdsaFromPemPublicKey(Convert.FromBase64String(publicKey.PublicKeyString));
				using var sha = SHA256.Create();
				var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(message));

				if (!ecdsa.VerifyHash(hash, signatureBytes))
					throw new UnauthorizedAccessException("Incorrect signature parameter in request.");
			}
			else
			{
				throw new ArgumentException($"Unsupported sign algorithm '{publicKey.SignAlgo}'. Maybe you must update the Target365.Sdk nuget package?");
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

		private string Serialize<T>(T item)
		{
			return JsonSerializer.Serialize(item, _jsonSerializerOptions);
		}

		private T Deserialize<T>(string json)
		{
			return JsonSerializer.Deserialize<T>(json, _jsonSerializerOptions);
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
				using var sha256 = SHA256.Create();
				var contentBytes = await request.Content.ReadAsByteArrayAsync().ConfigureAwait(false);

				if (contentBytes?.Length > 0)
					contentHash = Convert.ToBase64String(sha256.ComputeHash(contentBytes));
			}

			var message = $"{method}{uri}{timestamp}{nonce}{contentHash}";
			using var ecdsa = CryptoUtils.GetEcdsaFromPrivateKey(_cngPrivateKeyBytes);
			using var sha = SHA256.Create();
			var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(message));
			var signatureString = Convert.ToBase64String(ecdsa.SignHash(hash));
			request.Headers.Authorization = new AuthenticationHeaderValue("HMAC", $"{keyName}:{timestamp}:{nonce}:{signatureString}");
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
				var httpError = JsonSerializer.Deserialize<ErrorHolder>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
				message = httpError?.Message;
			}
			catch
			{
			}

			throw new ServiceClientException(message, request, response);
		}
	}
}
