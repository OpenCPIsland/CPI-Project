using DevonLocalization.Core;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

namespace Disney.Native
{
	public class ScrollerAccessibilitySettings : AccessibilitySettings
	{
		public bool ScrollBarOnly = false;

		public ScrollRect ReferenceScrollRect;

		public Scrollbar Scrollbar;

		private void Start()
		{
			Setup();
		}

		public override void Setup()
		{
			if (ReferenceScrollRect == null)
			{
				ReferenceScrollRect = GetComponentInParent<ScrollRect>();
			}
			if (Scrollbar == null && ReferenceScrollRect != null)
			{
				Scrollbar = ReferenceScrollRect.GetComponent<Scrollbar>();
				if (Scrollbar == null)
				{
					Scrollbar = ReferenceScrollRect.gameObject.GetComponentInChildren<Scrollbar>(true);
				}
			}
			base.Setup();
		}

		public string GetScrollPercent()
		{
			if (ReferenceScrollRect == null)
			{
				return "";
			}
			if (ReferenceScrollRect.vertical)
			{
				return Mathf.Round(ReferenceScrollRect.verticalNormalizedPosition * 100f) + "%";
			}
			if (ReferenceScrollRect.horizontal)
			{
				return Mathf.Round(ReferenceScrollRect.horizontalNormalizedPosition * 100f) + "%";
			}
			return "";
		}

		public string GetScrollText(int aIncrement)
		{
			return "";
		}

		public void SayScrollText(int aIncrement)
		{
			string scrollText = GetScrollText(aIncrement);
			string tokenTranslation = Service.Get<Localizer>().GetTokenTranslation(CustomToken);
			tokenTranslation = tokenTranslation.Replace("#age#", scrollText);
			tokenTranslation = tokenTranslation.Replace("#month#", scrollText);
			tokenTranslation = tokenTranslation.Replace("#day#", scrollText);
			MonoSingleton<NativeAccessibilityManager>.Instance.Native.Speak(tokenTranslation);
		}

		public string GetSayScrollText(int aIncrement)
		{
			string scrollText = GetScrollText(aIncrement);
			string tokenTranslation = Service.Get<Localizer>().GetTokenTranslation(CustomToken);
			tokenTranslation = tokenTranslation.Replace("#age#", scrollText);
			tokenTranslation = tokenTranslation.Replace("#month#", scrollText);
			return tokenTranslation.Replace("#day#", scrollText);
		}

		public void Scroll(float aDistance)
		{
			if (Scrollbar != null)
			{
				Rect rectInPhysicalScreenSpace = Util.GetRectInPhysicalScreenSpace(ReferenceScrollRect.gameObject.GetComponent<RectTransform>());
				Rect rectInPhysicalScreenSpace2 = Util.GetRectInPhysicalScreenSpace(ReferenceScrollRect.content);
				float num = ReferenceScrollRect.horizontal ? rectInPhysicalScreenSpace.width : rectInPhysicalScreenSpace.height;
				float num2 = ReferenceScrollRect.horizontal ? rectInPhysicalScreenSpace2.width : rectInPhysicalScreenSpace2.height;
				float num3 = aDistance * (num / num2);
				Scrollbar.value += num3;
			}
		}
	}
}
