using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.IO.Pem;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace DCPI.Platforms.SwrveManager.Utils
{
	public static class SwrveManagerUtils
	{
		private const int KEY_SIZE = 256;

		private const int ITERATIONS = 32768;

		private const string CIPHER_ALOGRITHM = "AES/CBC/PKCS5Padding";

		private const string KEY_ALOGRITHM = "PBKDF2WithHmacSHA1";

		private const string KEY_SPEC_ALOGRITHM = "AES";

		private const string ENCODING = "UTF-8";

		private static readonly string ENCRYPTION_ALGORITHM;

		private static readonly string ANDI_TYPE;

		private static KeyParameter secretKeyParameter;

		private static string saltString;

		private static Type andiType;

		public static Type ANDIType
		{
			get
			{
				return andiType;
			}
		}

		public static KeyParameter aesKey
		{
			get
			{
				return secretKeyParameter;
			}
		}

		static SwrveManagerUtils()
		{
			ENCRYPTION_ALGORITHM = "AES";
			ANDI_TYPE = "Disney.ANDI.ANDI";
			secretKeyParameter = null;
			saltString = string.Empty;
			andiType = null;
		}

		public static string GetIsJailBroken()
		{
			string result = string.Empty;
			if (Application.platform == RuntimePlatform.Android)
			{
				result = SwrveManagerUtilsAndroid.GetIsJailBroken();
			}
			else if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				result = SwrveManagerUtilsiOS.GetIsJailBroken();
			}
			return result;
		}

		public static string GetIsLat()
		{
			string result = string.Empty;
			if (Application.platform == RuntimePlatform.Android)
			{
				result = SwrveManagerUtilsAndroid.GetIsLat().ToString();
			}
			else if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				result = SwrveManagerUtilsiOS.GetIsLat().ToString();
			}
			return result;
		}

		public static string GetAdvertiserID()
		{
			string result = string.Empty;
			if (Application.platform == RuntimePlatform.Android)
			{
				result = SwrveManagerUtilsAndroid.GetGIDA();
			}
			else if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				result = SwrveManagerUtilsiOS.GetIDFA();
			}
			return result;
		}

		public static bool IsAndiAvailable()
		{
			if (andiType != null)
			{
				return true;
			}
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			int i = 0;
			for (int num = assemblies.Length; i < num; i++)
			{
				Assembly assembly = assemblies[i];
				andiType = assembly.GetType(ANDI_TYPE);
				if (andiType != null)
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsAndiInitialized()
		{
			bool result = false;
			if (andiType == null)
			{
				return result;
			}
			string value = (string)andiType.GetMethod("GetAndiu").Invoke(null, null);
			if (!string.IsNullOrEmpty(value))
			{
				result = true;
			}
			return result;
		}

		public static string AESEncrypt(string saltString, string plainText)
		{
			if (secretKeyParameter == null)
			{
				secretKeyParameter = KeyGen(saltString);
			}
			string empty = string.Empty;
			IBufferedCipher cipher = CipherUtilities.GetCipher(ENCRYPTION_ALGORITHM + "/CBC/PKCS5PADDING");
			byte[] array = new byte[cipher.GetBlockSize()];
			SecureRandom instance = SecureRandom.GetInstance("SHA1PRNG");
			instance.NextBytes(array);
			ParametersWithIV parametersWithIV = new ParametersWithIV(secretKeyParameter, array);
			cipher.Init(true, parametersWithIV);
			int num = array.Length;
			byte[] array2 = cipher.DoFinal(Encoding.UTF8.GetBytes(plainText));
			Debug.Log("AESEncrypt:: IV as string: " + Convert.ToBase64String(parametersWithIV.GetIV()));
			Debug.Log("AESEncrypt:: encryptedByte as string: " + Convert.ToBase64String(array2));
			byte[] array3 = new byte[num + array2.Length];
			Array.Copy(parametersWithIV.GetIV(), 0, array3, 0, num);
			Array.Copy(array2, 0, array3, num, array2.Length);
			empty = Convert.ToBase64String(array3, Base64FormattingOptions.None);
			Debug.Log("AESEncrypt:: encryptedString: " + empty);
			return empty;
		}

		public static string GetRSAEncryptedKey()
		{
			string result = string.Empty;
			if (secretKeyParameter == null)
			{
				Debug.LogError("### SwrveManagerUtils::GetRSAEncryptedKey: secretKeyParameter is null! There is nothing to encrypt");
				return result;
			}
			try
			{
				string text = (Resources.Load("pub") as TextAsset).text;
				text = text.Replace("-----BEGIN PUBLIC KEY-----", "").Replace("-----END PUBLIC KEY-----", "");
				result = RSAEncrypt(text, secretKeyParameter);
			}
			catch (Exception ex)
			{
				Debug.LogError("### SwrveManagerUtils::GetRSAEncryptedKey: " + ex.Message);
			}
			return result;
		}

		public static string RSAEncrypt(string pemStreamText, KeyParameter secretKeyParameter)
		{
			string result = string.Empty;
			try
			{
				RsaKeyParameters rsaKeyParameters = null;
				StreamReader reader = new StreamReader(new MemoryStream(Convert.FromBase64String(pemStreamText)));
				Org.BouncyCastle.OpenSsl.PemReader pemReader = new Org.BouncyCastle.OpenSsl.PemReader(reader);
				PemObject pemObject = pemReader.ReadPemObject();
				if (pemObject != null)
				{
					AsymmetricKeyParameter asymmetricKeyParameter = PublicKeyFactory.CreateKey(pemObject.Content);
					rsaKeyParameters = (RsaKeyParameters)asymmetricKeyParameter;
				}
				else
				{
					rsaKeyParameters = (RsaKeyParameters)PublicKeyFactory.CreateKey(Convert.FromBase64String(pemStreamText));
				}
				byte[] key = secretKeyParameter.GetKey();
				IBufferedCipher cipher = CipherUtilities.GetCipher("RSA/ECB/OAEPWithSHA_1AndMGF1Padding");
				cipher.Init(true, rsaKeyParameters);
				byte[] inArray = BlockCipher(key, cipher, true);
				result = Convert.ToBase64String(inArray, Base64FormattingOptions.None);
			}
			catch (Exception ex)
			{
				Debug.LogError("### SwrveManagerUtils::RSAEncrypt: " + ex.Message);
			}
			return result;
		}

		public static byte[] BlockCipher(byte[] bytes, IBufferedCipher cipher, bool isEncrypt)
		{
			if ((bytes.Length <= 62 && isEncrypt) || (bytes.Length <= 128 && !isEncrypt))
			{
				return cipher.DoFinal(bytes);
			}
			byte[] array = new byte[0];
			byte[] prefix = new byte[0];
			int num = isEncrypt ? 62 : 128;
			byte[] array2 = new byte[num];
			for (int i = 0; i < bytes.Length; i++)
			{
				if (i > 0 && i % num == 0)
				{
					array = cipher.DoFinal(array2);
					prefix = AppendBytes(prefix, array);
					int num2 = num;
					if (i + num > bytes.Length)
					{
						num2 = bytes.Length - i;
					}
					array2 = new byte[num2];
				}
				array2[i % num] = bytes[i];
			}
			array = cipher.DoFinal(array2);
			return AppendBytes(prefix, array);
		}

		public static byte[] AppendBytes(byte[] prefix, byte[] suffix)
		{
			byte[] array = new byte[prefix.Length + suffix.Length];
			for (int i = 0; i < prefix.Length; i++)
			{
				array[i] = prefix[i];
			}
			for (int i = 0; i < suffix.Length; i++)
			{
				array[i + prefix.Length] = suffix[i];
			}
			return array;
		}

		private static KeyParameter KeyGen(string salt)
		{
			string text = Guid.NewGuid().ToString();
			saltString = salt;
			byte[] bytes = Encoding.UTF8.GetBytes(saltString);
			char[] password = text.ToCharArray();
			byte[] password2 = PbeParametersGenerator.Pkcs12PasswordToBytes(password);
			IDigest digest = new Sha1Digest();
			PbeParametersGenerator pbeParametersGenerator = new Pkcs12ParametersGenerator(digest);
			pbeParametersGenerator.Init(password2, bytes, 32768);
			return (KeyParameter)pbeParametersGenerator.GenerateDerivedParameters("AES", 256);
		}
	}
}
