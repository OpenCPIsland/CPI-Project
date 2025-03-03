using ClubPenguin.Analytics;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Runtime.InteropServices;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class InAppPurchaseDisclaimerController : MonoBehaviour
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct ShowInAppPurchaseDisclaimerEvent
		{
		}

		public const string titleToken = "GlobalUI.IAPWarning.Apple.Title";

		public const string bodyToken = "GlobalUI.IAPWarning.Apple.Body";

		private readonly SpriteContentKey iconContentKey = new SpriteContentKey("Images/Prompt_Purchase");

		private void Start()
		{
			if (!Service.Get<GameSettings>().SeenInAppPurchaseDisclaimerPrompt.Value)
			{
				Content.LoadAsync(onIconLoaded, iconContentKey);
			}
		}

		private void onIconLoaded(string path, Sprite icon)
		{
			DPrompt data = new DPrompt("GlobalUI.IAPWarning.Apple.Title", "GlobalUI.IAPWarning.Apple.Body", DPrompt.ButtonFlags.OK, icon);
			Service.Get<EventDispatcher>().DispatchEvent(default(ShowInAppPurchaseDisclaimerEvent));
			Service.Get<PromptManager>().ShowPrompt(data, onButtonClicked);
			Service.Get<ICPSwrveService>().Action("purchase_disclaimer", "viewed");
		}

		private void onButtonClicked(DPrompt.ButtonFlags pressed)
		{
			Service.Get<GameSettings>().SeenInAppPurchaseDisclaimerPrompt.Value = true;
			Service.Get<ICPSwrveService>().Action("purchase_disclaimer", "accepted");
		}
	}
}
