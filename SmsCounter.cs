using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Target365.Sdk
{
	/// <summary>
	/// Utility class for counting sms message parts.
	/// </summary>
	public static class SmsCounter
	{
		private static readonly HashSet<char> _gsm7Chars = new(new[] { '@', '£', '$', '¥', 'è', 'é', 'ù', 'ì', 'ò', 'Ç', '\n', 'Ø', 'ø', '\r', 'Å', 'å', 'Δ', '_', 'Φ', 'Γ', 'Λ', 'Ω', 'Π', 'Ψ', 'Σ', 'Θ', 'Ξ', 'Æ', 'æ', 'ß', 'É', ' ', '!', '"', '#', '¤', '%', '&', '\'', '(', ')', '*', '+', ',', '-', '.', '/', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', ':', ';', '<', '=', '>', '?', '¡', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'Ä', 'Ö', 'Ñ', 'Ü', '§', '¿', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'ä', 'ö', 'ñ', 'ü', 'à' });
		private static readonly HashSet<char> _gsm7ExtendedChars = new(new[] { '\f', '^', '{', '}', '\\', '[', '~', ']', '|', '€' });
		private static readonly Regex _unmaskRegex = new(@"~~([\s\S]+?)~~", RegexOptions.Compiled);

		/// <summary>
		/// Gets the sms message parts required for the given out-message.
		/// </summary>
		/// <param name="outMessage">Out-message to test.</param>
		/// <exception cref="ArgumentNullException">Out-message is null.</exception>
		public static int GetSmsMessageParts(OutMessage outMessage)
		{
			_ = outMessage ?? throw new ArgumentNullException(nameof(outMessage));

			if (outMessage.Content == null)
				return 1;

			var unmaskedText = UnmaskText(outMessage.Content);

			if (outMessage.AllowUnicode != false && HasInvalidGsm7Chars(unmaskedText))
				return GetUcs2SmsParts(unmaskedText);
			else
				return GetGsm7SmsParts(unmaskedText);
		}

		/// <summary>
		/// Gets all non-GSM7 characters (like an emoji character) for the given out-message.
		/// </summary>
		/// <param name="outMessage">Out-message to test.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException">Out-message is null.</exception>
		public static char[] GetNonGsm7Characters(OutMessage outMessage)
		{
			_ = outMessage ?? throw new ArgumentNullException(nameof(outMessage));

			if (outMessage.Content == null)
				return Array.Empty<char>();

			var unmaskedText = UnmaskText(outMessage.Content);
			return unmaskedText.Where(x => !_gsm7Chars.Contains(x) && !_gsm7ExtendedChars.Contains(x)).ToArray();
		}

		private static string UnmaskText(string text)
		{
			if (string.IsNullOrEmpty(text))
				return text;

			return _unmaskRegex.Replace(text, m => m.Groups[1].Value);
		}

		private static int GetUcs2SmsParts(string text)
		{
			const int maxCharsPerPart = 67;

			if (text.Length <= 70)
				return 1;

			var count = 1;
			var chars = 0;

			for (var i = 0; i < text.Length; i++)
			{
				if (chars == maxCharsPerPart || (chars == (maxCharsPerPart - 1) && text[i] >= '\ud800' && text[i] <= '\udbff'))
				{
					count++;
					chars = 0;
				}

				chars++;
			}

			return count;
		}

		private static int GetGsm7SmsParts(string text)
		{
			const int maxBytesPerMessage = 140;
			const int maxSeptetsPerPart = 153;

			if (GetGsm7CharByteCount(text) <= maxBytesPerMessage)
				return 1;

			var count = 1;
			var septets = 0;

			for (var i = 0; i < text.Length; i++)
			{
				if (septets == maxSeptetsPerPart || (septets == (maxSeptetsPerPart - 1) && _gsm7ExtendedChars.Contains(text[i])))
				{
					count++;
					septets = 0;
				}

				if (_gsm7ExtendedChars.Contains(text[i]))
					septets++;

				septets++;
			}

			return count;
		}

		private static int GetGsm7CharByteCount(string chars)
		{
			int septets = 0;

			foreach (char c in chars)
			{
				if (_gsm7Chars.Contains(c))
				{
					septets++;
				}
				else
				{
					if (_gsm7ExtendedChars.Contains(c))
					{
						septets += 2;
					}
					else
					{
						septets++;
					}
				}
			}

			var bytes = septets * 7 / 8;
			var remainder = septets * 7 % 8 > 0 ? 1 : 0;
			return bytes + remainder;
		}

		private static bool HasInvalidGsm7Chars(string value)
		{
			return value.Any(x => !_gsm7Chars.Contains(x) && !_gsm7ExtendedChars.Contains(x));
		}
	}
}