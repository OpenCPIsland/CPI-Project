using Tweaker.Core;
using UnityEngine;

namespace ClubPenguin
{
	public static class DisplayResolutionManager
	{
		public const int MinimumWidth = 640;

		public const int MinimumHeight = 480;

		[Invokable("Debug.Resolution.SetRawResolution")]
		public static void SetRawResolution(int width = 1920, int height = 1080, bool fullscreen = true)
		{
			bool flag = true;
			if (!fullscreen)
			{
				flag = (height <= Screen.currentResolution.height && width <= Screen.currentResolution.width);
			}
			if (width >= 640 && height >= 480 && flag)
			{
				Screen.SetResolution(width, height, fullscreen);
			}
			else if (!flag)
			{
				Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, false);
			}
		}

		public static void SetResolution(Resolution resolution, bool fullscreen)
		{
			SetRawResolution(resolution.width, resolution.height, fullscreen);
		}
	}
}
