using Disney.Kelowna.Common;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class SnowballUIController : AvatarPositionTranslator
	{
		public PrefabContentKey SnowballPowerMeterKey;

		private RectTransform powerMeter;

		private bool isDestroyed;

		protected override void startInit()
		{
			Content.LoadAsync(onPowerMeterLoaded, SnowballPowerMeterKey);
		}

		private void OnDestroy()
		{
			isDestroyed = true;
		}

		private void Update()
		{
			if (powerMeter != null && powerMeter.gameObject.activeSelf)
			{
				localPlayer = getAvatar(base.localSessionId);
				powerMeter.anchoredPosition = getScreenPoint(localPlayer.position);
			}
		}

		private void onPowerMeterLoaded(string path, GameObject prefab)
		{
			if (!isDestroyed)
			{
				powerMeter = Object.Instantiate(prefab, base.transform, false).GetComponent<RectTransform>();
			}
		}
	}
}
