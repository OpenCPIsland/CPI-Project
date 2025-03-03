using Disney.Kelowna.Common;
using System.Collections;
using UnityEngine;

namespace ClubPenguin
{
	public class WindowResizeService : MonoBehaviour
	{
		private int width = 0;

		private int height = 0;

		private float desiredAspectRatio;

		private int previousScreensize = Screen.width * Screen.height;

		private bool isWindowResizing = false;

		private float osxResizeTime = 1f;

		private float winResizeTime = 0.6f;

		private WaitForSecondsRealtime waitForResize;

		private void Start()
		{
			waitForResize = new WaitForSecondsRealtime(winResizeTime);
		}

		private void Update()
		{
			if (!Screen.fullScreen)
			{
				float num = (float)Screen.width / (float)Screen.height;
				if ((Mathf.Abs(num - CustomGraphicsService.AspectRatio) > 0.01f || Screen.height < 480) && !isWindowResizing)
				{
					onWindowResize(Screen.width, Screen.height, CustomGraphicsService.AspectRatio);
				}
			}
		}

		private void onWindowResize(int _width, int _height, float _aspect)
		{
			if (_width != width || _height != height)
			{
				width = _width;
				height = _height;
				desiredAspectRatio = _aspect;
				CoroutineRunner.StopAllForOwner(this);
				CoroutineRunner.Start(resizeAfterDrag(), this, "ResizeAfterDrag");
			}
		}

		private IEnumerator resizeAfterDrag()
		{
			int waitFrames = 20;
			int currentFrame = 0;
			while (currentFrame < waitFrames)
			{
				currentFrame++;
				yield return null;
			}
			float currentAspectRatio = (float)width / (float)height;
			int screenSize = width * height;
			if (screenSize >= previousScreensize)
			{
				if (currentAspectRatio > desiredAspectRatio)
				{
					height = (int)((float)width / desiredAspectRatio);
				}
				else
				{
					width = (int)((float)height * desiredAspectRatio);
				}
				DisplayResolutionManager.SetRawResolution(width, height, false);
			}
			else
			{
				if (currentAspectRatio > desiredAspectRatio)
				{
					width = (int)((float)height * desiredAspectRatio);
				}
				else
				{
					height = (int)((float)width / desiredAspectRatio);
				}
				if (width <= 640 && height <= 480)
				{
					height = 480;
					width = (int)((float)height * desiredAspectRatio);
				}
				DisplayResolutionManager.SetRawResolution(width, height, false);
			}
			isWindowResizing = true;
			yield return waitForResize;
			isWindowResizing = false;
			previousScreensize = width * height;
		}
	}
}
