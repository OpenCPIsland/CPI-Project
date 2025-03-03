using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.CellPhone
{
	public class CellPhoneReceptionBars : MonoBehaviour
	{
		private const int OFF_SPRITE_INDEX = 0;

		private const int ON_SPRITE_INDEX = 1;

		private const float DEFAULT_HEIGHT_OFFSET = 14.5f;

		private const float DEFAULT_HEIGHT_DIVIDER = 24f;

		public SpriteSelector[] Bars;

		public CellPhoneReceptionBarZoneInfo[] ZoneInfoList;

		private void Start()
		{
			if (SceneRefs.ZoneLocalPlayerManager == null || SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject == null)
			{
				return;
			}
			GameObject localPlayerGameObject = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject;
			string zoneName = Service.Get<ZoneTransitionService>().CurrentZone.ZoneName;
			CellPhoneReceptionBarZoneInfo cellPhoneReceptionBarZoneInfo = default(CellPhoneReceptionBarZoneInfo);
			cellPhoneReceptionBarZoneInfo.HeightOffset = 14.5f;
			cellPhoneReceptionBarZoneInfo.HeightDivider = 24f;
			for (int i = 0; i < ZoneInfoList.Length; i++)
			{
				if (ZoneInfoList[i].ZoneName == zoneName)
				{
					cellPhoneReceptionBarZoneInfo = ZoneInfoList[i];
				}
			}
			float value = (localPlayerGameObject.transform.position.y + cellPhoneReceptionBarZoneInfo.HeightOffset) / cellPhoneReceptionBarZoneInfo.HeightDivider;
			value = Mathf.Clamp(value, 0f, 1f);
			UpdateBarGraphics(value);
		}

		private void UpdateBarGraphics(float percentFull)
		{
			int num = Mathf.FloorToInt((float)Bars.Length * percentFull);
			for (int i = 0; i < Bars.Length; i++)
			{
				if (i == 0 || i < num)
				{
					Bars[i].SelectSprite(1);
				}
				else
				{
					Bars[i].SelectSprite(0);
				}
			}
		}
	}
}
