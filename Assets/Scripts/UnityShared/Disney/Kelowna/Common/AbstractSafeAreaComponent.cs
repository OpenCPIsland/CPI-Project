using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;

namespace Disney.Kelowna.Common
{
	public abstract class AbstractSafeAreaComponent : MonoBehaviour
	{
		protected SafeAreaService safeAreaService;

		private void Awake()
		{
			if (Service.IsSet<SafeAreaService>())
			{
				safeAreaService = Service.Get<SafeAreaService>();
				return;
			}
			base.enabled = false;
			StartCoroutine(waitForService());
		}

		private IEnumerator waitForService()
		{
			while (!Service.IsSet<SafeAreaService>())
			{
				yield return null;
			}
			safeAreaService = Service.Get<SafeAreaService>();
			base.enabled = true;
		}

		protected float getVerticalOffset(SafeArea safeArea)
		{
			RectOffset safeAreaOffset = safeAreaService.GetSafeAreaOffset();
			switch (safeArea)
			{
			case SafeArea.Top:
				return safeAreaOffset.top;
			case SafeArea.Bottom:
				return safeAreaOffset.bottom;
			default:
				return 0f;
			}
		}
	}
}
