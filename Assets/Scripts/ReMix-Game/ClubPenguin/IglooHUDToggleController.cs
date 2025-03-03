using ClubPenguin.UI;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ClubPenguin
{
	public class IglooHUDToggleController : MonoBehaviour
	{
		public GameObject hudRootObject;

		public GameObject[] enableInOwnIgloo;

		public GameObject[] enableInRemoteIgloo;

		private EventDispatcher eventDispatcher;

		private void Awake()
		{
			bool flag = SceneManager.GetActiveScene().name != "ClothingDesigner" && Service.Get<ZoneTransitionService>().IsInIgloo;
			bool flag2 = Service.Get<SceneLayoutDataManager>().IsInOwnIgloo();
			for (int i = 0; i < enableInOwnIgloo.Length; i++)
			{
				enableInOwnIgloo[i].SetActive(flag && flag2);
			}
			if (flag && flag2)
			{
				eventDispatcher = Service.Get<EventDispatcher>();
				eventDispatcher.AddListener<HudEvents.HideIglooEditButton>(onHideIglooEditButton);
				eventDispatcher.AddListener<HudEvents.ShowIglooEditButton>(onShowIglooEditButton);
			}
			for (int i = 0; i < enableInRemoteIgloo.Length; i++)
			{
				enableInRemoteIgloo[i].SetActive(flag && !flag2);
			}
			hudRootObject.SetActive(flag);
		}

		private void OnDestroy()
		{
			if (eventDispatcher != null)
			{
				eventDispatcher.RemoveListener<HudEvents.HideIglooEditButton>(onHideIglooEditButton);
				eventDispatcher.RemoveListener<HudEvents.ShowIglooEditButton>(onShowIglooEditButton);
			}
		}

		private bool onHideIglooEditButton(HudEvents.HideIglooEditButton evt)
		{
			for (int i = 0; i < enableInOwnIgloo.Length; i++)
			{
				enableInOwnIgloo[i].SetActive(false);
			}
			return false;
		}

		private bool onShowIglooEditButton(HudEvents.ShowIglooEditButton evt)
		{
			for (int i = 0; i < enableInOwnIgloo.Length; i++)
			{
				enableInOwnIgloo[i].SetActive(true);
			}
			return false;
		}
	}
}
