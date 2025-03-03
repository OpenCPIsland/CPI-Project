using UnityEngine;

namespace ClubPenguin.Rewards
{
	public class RewardPopupBG : MonoBehaviour
	{
		private const string MAIN_NAV_NAME = "MainNavBar";

		public void Start()
		{
			RectTransform component = GameObject.Find("MainNavBar").GetComponent<RectTransform>();
			float y = component.GetComponentInParent<Canvas>().GetComponent<RectTransform>().sizeDelta.y;
			float y2 = GetComponentInParent<Canvas>().GetComponent<RectTransform>().sizeDelta.y;
			float num = y / component.offsetMax.y;
			float y3 = y2 / num;
			GetComponent<RectTransform>().offsetMin = new Vector2(0f, y3);
		}
	}
}
