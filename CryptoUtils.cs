using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace Target365.Sdk
{
	internal static class CryptoUtils
	{
		private static readonly byte[] pemP256PublicKeyPrefix = new byte[]
		{
			0x30, 0x59, 0x30, 0x13, 0x06, 0x07, 0x2A, 0x86, 0x48, 0xCE, 0x3D, 0x02, 0x01, 0x06, 0x08,
			0x2A, 0x86, 0x48, 0xCE, 0x3D, 0x03, 0x01, 0x07, 0x03, 0x42, 0x00, 0x04
		};

		private static readonly byte[] pemP521PublicKeyPrefix = new byte[]
		{
			0x30, 0x81, 0x9b, 0x30, 0x10, 0x06, 0x07, 0x2a, 0x86, 0x48, 0xce, 0x3d, 0x02, 0x01, 0x06,
			0x05, 0x2b, 0x81, 0x04, 0x00, 0x23, 0x03, 0x81, 0x86, 0x00, 0x04
		};

		private static readonly byte[] pemRsa1024PublicKeyPrefix = new byte[]
		{
			0x30, 0x81, 0x9F, 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01,
			0x01, 0x05, 0x00, 0x03, 0x81, 0x8D, 0x00, 0x30, 0x81, 0x89, 0x02, 0x81, 0x81, 0x00
		};

		private static readonly byte[] pemRsa1024PublicKeySuffix = new byte[]
		{
			0x02, 0x03, 0x01, 0x00, 0x01
		};

		private static readonly byte[] pemRsa2048PublicKeyPrefix = new byte[]
		{
			0x30, 0x82, 0x01, 0x22, 0x30, 0x0d, 0x06, 0x09, 0x2a, 0x86, 0x48, 0x86, 0xf7, 0x0d, 0x01,
			0x01, 0x01, 0x05, 0x00, 0x03, 0x82, 0x01, 0x0f, 0x00, 0x30, 0x82, 0x01, 0x0a, 0x02, 0x82,
			0x01, 0x01, 0x00,
		};

		private static readonly byte[] pemRsa2048PublicKeySuffix = new byte[]
		{
			0x02, 0x03, 0x01, 0x00, 0x01,
		};

		private static readonly byte[] cngP256PublicKeyPrefix = new byte[]
		{
			0x45, 0x43, 0x53, 0x31, 0x20, 0x00, 0x00, 0x00
		};

		private static readonly byte[] cngP521PublicKeyPrefix = new byte[]
		{
			0x45, 0x43, 0x53, 0x35, 0x42, 0x00, 0x00, 0x00
		};

		private static readonly byte[] cngRsa1024PublicKeyPrefix = new byte[]
		{
			0x52, 0x53, 0x41, 0x31, 0x00, 0x04, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00, 0x80, 0x00, 0x00,
			0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x01
		};

		private static readonly byte[] cngRsa2048PublicKeyPrefix = new byte[]
		{
			0x52, 0x53, 0x41, 0x31, 0x00, 0x08, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00,
			0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x01
		};

		public const string ECDsaP256 = "ECDsaP256";
		public const string ECDsaP521 = "ECDsaP521";
		public const string Rsa = "Rsa";

		public static byte[] GetRawKey(byte[] keyData)
		{
			_ = keyData ?? throw new ArgumentNullException(nameof(keyData));

			if (StartsWith(keyData, pemP256PublicKeyPrefix))
			{
				return keyData.Skip(pemP256PublicKeyPrefix.Length).ToArray();
			}
			else if (StartsWith(keyData, pemP521PublicKeyPrefix))
			{
				return keyData.Skip(pemP521PublicKeyPrefix.Length).ToArray();
			}
			else if (StartsWith(keyData, pemRsa1024PublicKeyPrefix))
			{
				return keyData.Skip(pemRsa1024PublicKeyPrefix.Length).Take(128).ToArray();
			}
			else if (StartsWith(keyData, pemRsa2048PublicKeyPrefix))
			{
				return keyData.Skip(pemRsa2048PublicKeyPrefix.Length).Take(256).ToArray();
			}
			else if (StartsWith(keyData, cngP256PublicKeyPrefix))
			{
				return keyData.Skip(cngP256PublicKeyPrefix.Length).ToArray();
			}
			else if (StartsWith(keyData, cngP521PublicKeyPrefix))
			{
				return keyData.Skip(cngP521PublicKeyPrefix.Length).ToArray();
			}
			else if (StartsWith(keyData, cngRsa1024PublicKeyPrefix))
			{
				return keyData.Skip(cngRsa1024PublicKeyPrefix.Length).ToArray();
			}
			else if (StartsWith(keyData, cngRsa2048PublicKeyPrefix))
			{
				return keyData.Skip(cngRsa2048PublicKeyPrefix.Length).ToArray();
			}

			throw new ArgumentException("Unsupported PEM format. Only P256, P521, RSA-1024 and RSA-2048 public keys are supported.");
		}

		public static byte[] GetPemBytes(byte[] rawKey, string algorithm)
		{
			_ = rawKey ?? throw new ArgumentNullException(nameof(rawKey));

			if (algorithm == ECDsaP256)
			{
				return pemP256PublicKeyPrefix.Concat(rawKey).ToArray();
			}
			if (algorithm == ECDsaP521)
			{
				return pemP521PublicKeyPrefix.Concat(rawKey).ToArray();
			}
			else if (algorithm == Rsa && rawKey.Length == 128)
			{
				return pemRsa1024PublicKeyPrefix.Concat(rawKey).Concat(pemRsa1024PublicKeySuffix).ToArray();
			}
			else if (algorithm == Rsa && rawKey.Length == 256)
			{
				return pemRsa2048PublicKeyPrefix.Concat(rawKey).Concat(pemRsa2048PublicKeySuffix).ToArray();
			}

			throw new ArgumentException("Unsupported raw key. Only P256, P521, RSA-1024 and RSA-2048 public keys are supported.");
		}

		public static byte[] GetCngBytes(byte[] rawKey, string algorithm)
		{
			_ = rawKey ?? throw new ArgumentNullException(nameof(rawKey));

			if (algorithm == ECDsaP256)
			{
				return cngP256PublicKeyPrefix.Concat(rawKey).ToArray();
			}
			if (algorithm == ECDsaP521)
			{
				return cngP521PublicKeyPrefix.Concat(rawKey).ToArray();
			}
			else if (algorithm == Rsa && rawKey.Length == 128)
			{
				return cngRsa1024PublicKeyPrefix.Concat(rawKey).ToArray();
			}
			else if (algorithm == Rsa && rawKey.Length == 256)
			{
				return cngRsa2048PublicKeyPrefix.Concat(rawKey).ToArray();
			}

			throw new ArgumentException("Unsupported raw key. Only ECDsaP256, ECDsaP521 and RSA is supported.");
		}

		public static ECDsa GetEcdsaFromPrivateKey(byte[] privateKey)
		{
#if NET461
			if (IsPemPrivateKey(privateKey))
				privateKey = PemPrivateKeyToCng(privateKey);

			return new ECDsaCng(CngKey.Import(privateKey, CngKeyBlobFormat.EccPrivateBlob));
#else
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				if (IsPemPrivateKey(privateKey))
					privateKey = PemPrivateKeyToCng(privateKey);

				return new ECDsaCng(CngKey.Import(privateKey, CngKeyBlobFormat.EccPrivateBlob));
			}
			else
			{
				var ecdsa = ECDsa.Create();
				var pemBytes = IsPemPrivateKey(privateKey) ? privateKey : GetPemBytesFromCngPrivateKeyBytes(privateKey);
				ecdsa.ImportPkcs8PrivateKey(pemBytes, out _);
				return ecdsa;
			}
#endif
		}

		private static bool IsPemPrivateKey(byte[] privateKey)
		{
			return privateKey[0] == 0x30 && privateKey.Length == 165;
		}

		public static byte[] PemPrivateKeyToCng(byte[] pemPrivateKey)
		{
			var prefix = new byte[] { 0x45, 0x43, 0x53, 0x32, 0x20, 0x00, 0x00, 0x00 };
			var d = pemPrivateKey.Skip(36).Take(32);
			var x = pemPrivateKey.Skip(36 + 32 + 18).Take(32);
			var y = pemPrivateKey.Skip(36 + 32 + 18 + 32).Take(32);
			return prefix.Concat(x).Concat(y).Concat(d).ToArray();
		}

		public static ECDsa GetEcdsaFromPemPrivateKey(byte[] pemPrivateKey)
		{
#if NET461
			var cngPrivateKey = GetCngBytesFromPemPrivateKeyBytes(pemPrivateKey);
			return new ECDsaCng(CngKey.Import(cngPrivateKey, CngKeyBlobFormat.EccPrivateBlob));
#else
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				var cngPrivateKey = GetCngBytesFromPemPrivateKeyBytes(pemPrivateKey);
				return new ECDsaCng(CngKey.Import(cngPrivateKey, CngKeyBlobFormat.EccPrivateBlob));
			}
			else
			{
				var ecdsa = ECDsa.Create();
				ecdsa.ImportPkcs8PrivateKey(pemPrivateKey, out _);
				return ecdsa;
			}
#endif
		}

		public static ECDsa GetEcdsaFromPemPublicKey(byte[] pemBytes)
		{
#if NET461
			var rawBytes = GetRawKey(pemBytes);
			var cngBytes = GetCngBytes(rawBytes, ECDsaP256);
			return new ECDsaCng(CngKey.Import(cngBytes, CngKeyBlobFormat.GenericPublicBlob));
#else
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				var rawBytes = GetRawKey(pemBytes);
				var cngBytes = GetCngBytes(rawBytes, ECDsaP256);
				return new ECDsaCng(CngKey.Import(cngBytes, CngKeyBlobFormat.GenericPublicBlob));
			}
			else
			{
				var ecdsa = ECDsa.Create();
				ecdsa.ImportSubjectPublicKeyInfo(pemBytes, out _);
				return ecdsa;
			}
#endif
		}

		private static byte[] GetPemBytesFromCngPrivateKeyBytes(byte[] cngEcPrivateBlob)
		{
			var derPrefix = new byte[] { 0x30, 0x81, 0xA2, 0x02, 0x01, 0x00, 0x30, 0x13, 0x06, 0x07, 0x2A, 0x86, 0x48, 0xCE, 0x3D, 0x02, 0x01, 0x06, 0x08, 0x2A, 0x86, 0x48, 0xCE, 0x3D, 0x03, 0x01, 0x07, 0x04, 0x79, 0x30, 0x77, 0x02, 0x01, 0x01, 0x04, 0x20 };
			var derMid = new byte[] { 0xA0, 0x0A, 0x06, 0x08, 0x2A, 0x86, 0x48, 0xCE, 0x3D, 0x03, 0x01, 0x07, 0xA1, 0x44, 0x03, 0x42, 0x00, 0x04 };
			var derPostfix = new byte[] { 0xA0, 0x0D, 0x30, 0x0B, 0x06, 0x03, 0x55, 0x1D, 0x0F, 0x31, 0x04, 0x03, 0x02, 0x00, 0x90 };
			var publicKey = cngEcPrivateBlob.Skip(8).Take(64);
			var privateKey = cngEcPrivateBlob.Skip(8 + 64).Take(32);
			return derPrefix.Concat(privateKey).Concat(derMid).Concat(publicKey).Concat(derPostfix).ToArray();
		}

		private static byte[] GetCngBytesFromPemPrivateKeyBytes(byte[] PemEcPrivateKey)
		{
			var cngPrefix = new byte[] { 0x45, 0x43, 0x53, 0x32, 0x20, 0x00, 0x00, 0x00 };
			var derPrefixLen = 36;
			var derMidLen = 18;
			var privateKey = PemEcPrivateKey.Skip(derPrefixLen).Take(32).ToArray();
			var publicKey = PemEcPrivateKey.Skip(derPrefixLen + 32 + derMidLen).Take(64).ToArray();
			return cngPrefix.Concat(publicKey).Concat(privateKey).ToArray();
		}

		private static bool StartsWith(byte[] data, byte[] prefix)
		{
			_ = data ?? throw new ArgumentNullException(nameof(data));
			_ = prefix ?? throw new ArgumentNullException(nameof(prefix));

			if (data.Length < prefix.Length)
				return false;

			for (var i = 0; i < prefix.Length; i++)
			{
				if (data[i] != prefix[i])
					return false;
			}

			return true;
		}
	}
}
