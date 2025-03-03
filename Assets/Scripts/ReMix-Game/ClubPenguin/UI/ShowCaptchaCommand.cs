using ClubPenguin.Net.Domain;
using Disney.Kelowna.Common;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class ShowCaptchaCommand
	{
		private readonly PrefabContentKey captchaUIContentKey = new PrefabContentKey("CaptchaPrefabs/CaptchaPrompt");

		public void Execute(CaptchaType captchaType, int? width = null, int? height = null)
		{
			Content.LoadAsync(delegate(string path, GameObject prefab)
			{
				onCaptchaUILoaded(prefab, captchaType, width, height);
			}, captchaUIContentKey);
		}

		private void onCaptchaUILoaded(GameObject captchaPrefab, CaptchaType captchaType, int? width = null, int? height = null)
		{
			GameObject gameObject = Object.Instantiate(captchaPrefab);
			CaptchaController component = gameObject.GetComponent<CaptchaController>();
			component.SetupCaptcha(captchaType, width, height);
		}
	}
}
