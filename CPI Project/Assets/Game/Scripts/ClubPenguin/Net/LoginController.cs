using ClubPenguin.Mix;
using Disney.MobileNetwork;
using Tweaker.Core;
using UnityEngine;

namespace ClubPenguin.Net
{
	public class LoginController
	{
		private MixLoginCreateService loginService;

		private GameObject popupRoot;

		public static bool SkipAutoLogin = false;

		[Tweakable("Session.SetAutoLogin")]
		public static bool AutoLoginEnabled
		{
			get
			{
				return Service.Get<GameSettings>().AutoLogin.Value;
			}
			set
			{
				Service.Get<GameSettings>().AutoLogin.SetValue(value);
			}
		}

		public LoginController()
		{
			loginService = Service.Get<MixLoginCreateService>();
		}

		public void SetNetworkConfig(NetworkServicesConfig config)
		{
			loginService.SetNetworkConfig(config);
			loginService.GetRegistrationConfig();
		}
	}
}
