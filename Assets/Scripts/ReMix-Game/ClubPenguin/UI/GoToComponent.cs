using ClubPenguin.Analytics;
using ClubPenguin.CellPhone;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class GoToComponent : MonoBehaviour
	{
		public ZoneDefinition Zone;

		public string SceneName;

		public Vector3 Position;

		public string SwrveLogTier1;

		private Button button;

		private void Start()
		{
			button = GetComponent<Button>();
			button.onClick.AddListener(onButtonClick);
		}

		private void OnDestroy()
		{
			if (button != null)
			{
				button.onClick.RemoveListener(onButtonClick);
			}
		}

		private void onButtonClick()
		{
			if (Object.FindObjectOfType<CellPhoneActivityScreenController>() != null)
			{
				Service.Get<EventDispatcher>().DispatchEvent(default(CellPhoneEvents.CellPhoneClosed));
			}
			PlayerSpawnPositionManager component = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject.GetComponent<PlayerSpawnPositionManager>();
			if (component != null)
			{
				component.SpawnPlayer(new SpawnPlayerParams.SpawnPlayerParamsBuilder(Position).Zone(Zone).SceneName(SceneName).Build());
			}
			if (!string.IsNullOrEmpty(SwrveLogTier1))
			{
				Service.Get<ICPSwrveService>().Action(SwrveLogTier1);
			}
		}
	}
}
