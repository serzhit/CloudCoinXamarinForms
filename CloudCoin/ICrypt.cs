namespace CloudCoin
{
	public interface ICrypt
	{
		byte[] Encrypt(string tobeEncrypted, string password, byte[] salt);
		string Decrypt(byte[] encryptedBytes, string password, byte[] salt);
	}
}
