using UnityEngine;

namespace Disney.MobileNetwork
{
	public class AccessibilityManager : MonoBehaviour
	{
		public virtual void Init()
		{
		}

		private void Awake()
		{
			Service.Set(this);
			Init();
		}

		public virtual float GetAdjustedFontSize(float aFontSize)
		{
			return aFontSize;
		}

		public virtual bool IsOtherAudioPlaying()
		{
			return false;
		}

		public virtual Vector2 GetScreenSize()
		{
			return new Vector2(Display.displays[0].systemWidth, Display.displays[0].systemHeight);
		}

		public virtual Vector2 GetScreenSizeWithSoftKeys()
		{
			return new Vector2(Display.displays[0].systemWidth, Display.displays[0].systemHeight);
		}

		public virtual void Speak(string aTextToSpeak, float rate)
		{
		}

		public virtual int GetStatusBarHeight()
		{
			return 0;
		}

		public virtual string GetProfileId()
		{
			return "";
		}
	}
}
