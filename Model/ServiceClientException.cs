using System;
using System.Net;
using System.Net.Http;

namespace Target365.Sdk
{
	/// <summary>
	/// Service client exception.
	/// </summary>
	public class ServiceClientException : Exception
	{
		/// <summary>
		/// Response http status code.
		/// </summary>
		public HttpStatusCode StatusCode { get { return Response.StatusCode; } }

		/// <summary>
		/// Response reason phrase.
		/// </summary>
		public string ReasonPhrase { get { return Response.ReasonPhrase; } }

		/// <summary>
		/// Request associated.
		/// </summary>
		public HttpRequestMessage Request { get; }

		/// <summary>
		/// Response associated.
		/// </summary>
		public HttpResponseMessage Response { get; }

		/// <summary>
		/// Whether the exception was based on client making a mistake or not.
		/// </summary>
		public bool IsClientException { get { return (int)StatusCode >= 400 && (int)StatusCode <= 499; } }

		/// <summary>
		/// Initializes a new ServiceClientException.
		/// </summary>
		/// <param name="message">Error message.</param>
		/// <param name="request">Request object.</param>
		/// <param name="response">Response object.</param>
		public ServiceClientException(string message, HttpRequestMessage request, HttpResponseMessage response)
			: base(message ?? $"API call resulting in http status {(int)response.StatusCode} and reason phrase: '{response.ReasonPhrase}'.")
		{
			Request = request ?? throw new ArgumentNullException(nameof(request));
			Response = response ?? throw new ArgumentNullException(nameof(response));
		}
	}
}
