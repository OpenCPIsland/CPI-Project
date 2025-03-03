using UnityEngine;

namespace ClubPenguin.Commerce
{
	public class CommerceProcessorInit
	{
		public static CommerceProcessor commerceProcessor;

		public CommerceProcessor GetCommerceProcessor(int testMode = 0, bool forceMock = false)
		{
			if (forceMock)
			{
				return getCommerceProcessorMock(testMode);
			}
			switch (Application.platform)
			{
			case RuntimePlatform.Android:
				commerceProcessor = new CommerceProcessorGooglePlay();
				break;
			case RuntimePlatform.IPhonePlayer:
				commerceProcessor = new CommerceProcessorApple();
				break;
			case RuntimePlatform.OSXPlayer:
			case RuntimePlatform.WindowsPlayer:
				commerceProcessor = new CommerceProcessorCSG();
				break;
			case RuntimePlatform.OSXEditor:
			case RuntimePlatform.WindowsEditor:
				commerceProcessor = new CommerceProcessorCSG();
				break;
			}
			if (commerceProcessor == null)
			{
				commerceProcessor = getCommerceProcessorMock(testMode);
			}
			return commerceProcessor;
		}

		private CommerceProcessor getCommerceProcessorMock(int testMode = 0)
		{
			commerceProcessor = new CommerceProcessorMock();
			commerceProcessor.SetTestMode(testMode);
			return commerceProcessor;
		}
	}
}
