namespace Disney.LaunchPad.Packages.Cryptography
{
	public interface ICipherStrategy
	{
		byte[] Encrypt(string unencryptedText);

		string Decrypt(byte[] encryptedData);
	}
}
