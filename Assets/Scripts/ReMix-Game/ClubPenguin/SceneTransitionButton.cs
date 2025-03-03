using Disney.MobileNetwork;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin
{
	[RequireComponent(typeof(Button))]
	public class SceneTransitionButton : MonoBehaviour
	{
		public string TargetScene;

		public string LoadingScene = "Loading";

		public List<string> SceneArgs = new List<string>(0);

		private Button button;

		public void Awake()
		{
			button = GetComponent<Button>();
		}

		public void OnEnable()
		{
			button.onClick.AddListener(Load);
		}

		public void OnDisable()
		{
			button.onClick.RemoveListener(Load);
		}

		public virtual void Load()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			if (SceneArgs.Count > 0)
			{
				foreach (string sceneArg in SceneArgs)
				{
					string[] array = sceneArg.Split('=');
					string key = array[0];
					string value = array[1];
					dictionary.Add(key, value);
				}
			}
			if (string.IsNullOrEmpty(TargetScene))
			{
				Service.Get<ZoneTransitionService>().LoadCurrentZone(LoadingScene);
			}
			else if (Service.Get<ZoneTransitionService>().GetZoneBySceneName(TargetScene) != null)
			{
				Service.Get<ZoneTransitionService>().LoadZone(TargetScene, LoadingScene);
			}
			else
			{
				Service.Get<SceneTransitionService>().LoadScene(TargetScene, LoadingScene, dictionary);
			}
		}
	}
}
