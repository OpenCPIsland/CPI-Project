namespace Disney.LaunchPad.Packages.Cryptography
{
	public class Cryptographer
	{
		private ICipherStrategy m_cipherStrategy = null;

		public Cryptographer(ICipherStrategy cipherStrategy)
		{
			m_cipherStrategy = cipherStrategy;
		}

		public byte[] Encrypt(string unencryptedText)
		{
			return m_cipherStrategy.Encrypt(unencryptedText);
		}

		public string Decrypt(byte[] encryptedData)
		{
			return m_cipherStrategy.Decrypt(encryptedData);
		}
	}
}
