using System;
using System.Security.Cryptography;

namespace CloudCoin.Droid
{
	public class CryptDroidImplementation : ICrypt
	{
		const int Rfc2898KeygenIterations = 100;
		const int AesKeySizeInBits = 128;

		public byte[] Encrypt(string tobeEncrypted, string password, byte[] salt)
		{
			byte[] cipherText;
			using (Aes aes = new AesManaged())
			{
				aes.Padding = PaddingMode.PKCS7;
				aes.KeySize = AesKeySizeInBits;
				int KeyStrengthInBytes = aes.KeySize / 8;
				Rfc2898DeriveBytes rfc2898 =
					new Rfc2898DeriveBytes(password, salt, Rfc2898KeygenIterations);
				aes.Key = rfc2898.GetBytes(KeyStrengthInBytes);
				aes.IV = rfc2898.GetBytes(KeyStrengthInBytes);
				using (MemoryStream ms = new MemoryStream())
				{
					using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
					{
						using (StreamWriter swEncrypt = new StreamWriter(cs))
						{
							swEncrypt.Write(tobeEncrypted);
						}
						cipherText = ms.ToArray();
					}
				}
			}
			return cipherText;
		}

		public string Decrypt(byte[] encryptedBytes, string password, byte[] salt)
		{
			string plainText = null;
			using (Aes aes = new AesManaged())
			{
				aes.Padding = PaddingMode.PKCS7;
				aes.KeySize = AesKeySizeInBits;
				int KeyStrengthInBytes = aes.KeySize / 8;
				Rfc2898DeriveBytes rfc2898 =
					new Rfc2898DeriveBytes(password, salt, Rfc2898KeygenIterations);
				aes.Key = rfc2898.GetBytes(KeyStrengthInBytes);
				aes.IV = rfc2898.GetBytes(KeyStrengthInBytes);
				using (MemoryStream ms = new MemoryStream(encryptedBytes))
				{
					using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read))
					{
						using (StreamReader srDecrypt = new StreamReader(cs))
						{
							plainText = srDecrypt.ReadToEnd();
						}
					}
				}
			}
			return plainText;
		}
	}
}
