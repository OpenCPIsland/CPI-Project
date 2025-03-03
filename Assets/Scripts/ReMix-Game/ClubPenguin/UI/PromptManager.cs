using ClubPenguin.Net;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class PromptManager : MonoBehaviour
	{
		public PromptController FatalPrefab;

		public PromptController ErrorPrefab;

		public PromptController PromptPrefab;

		public Action<GameObject> PromptCreated;

		private EventDispatcher dispatcher;

		private void Awake()
		{
			Service.Set(this);
		}

		private void OnValidate()
		{
		}

		private void OnDestroy()
		{
			if (dispatcher != null)
			{
				dispatcher.RemoveListener<SessionEvents.SessionPausedEvent>(onSessionPaused);
			}
		}

		public void SetEventDispatcher(EventDispatcher dispatcher)
		{
			this.dispatcher = dispatcher;
			this.dispatcher.AddListener<SessionEvents.SessionPausedEvent>(onSessionPaused);
		}

		public PromptDefinition GetPromptDefinition(string promptId)
		{
			Dictionary<string, PromptDefinition> dictionary = Service.Get<GameData>().Get<Dictionary<string, PromptDefinition>>();
			if (dictionary.ContainsKey(promptId))
			{
				return dictionary[promptId];
			}
			return null;
		}

		public void ShowFatal(string type, string format, params object[] args)
		{
			DPrompt.ButtonFlags buttons = DPrompt.ButtonFlags.None;
			DPrompt data = new DPrompt(type, string.Format(format, args), buttons);
			ShowPrompt(data, null, FatalPrefab);
		}

		public GameObject ShowError(string type, string message, Action<DPrompt.ButtonFlags> callback = null)
		{
			DPrompt data = new DPrompt(type, message, DPrompt.ButtonFlags.CLOSE);
			return ShowPrompt(data, callback, ErrorPrefab);
		}

		public GameObject ShowPrompt(DPrompt data, Action<DPrompt.ButtonFlags> callback, PromptController prefab = null)
		{
			if (prefab == null)
			{
				prefab = PromptPrefab;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(prefab.gameObject);
			gameObject.transform.SetParent(base.transform, false);
			PromptController component = gameObject.GetComponent<PromptController>();
			component.ShowPrompt(data, callback);
			if (PromptCreated != null)
			{
				PromptCreated(gameObject);
			}
			return gameObject;
		}

		public bool ShowPrompt(string promptId, Action<DPrompt.ButtonFlags> callback)
		{
			Dictionary<string, PromptDefinition> dictionary = Service.Get<GameData>().Get<Dictionary<string, PromptDefinition>>();
			if (dictionary.ContainsKey(promptId))
			{
				PromptLoaderCMD promptLoaderCMD = new PromptLoaderCMD(this, dictionary[promptId], showPrompt, callback);
				promptLoaderCMD.Execute();
				return true;
			}
			Log.LogError(this, string.Format("Could not find prompt definition: {0}", promptId));
			return false;
		}

		public void ShowPrompt(string promptId, string titleTextInsert, string bodyTextInsert, Action<DPrompt.ButtonFlags> callback)
		{
			Dictionary<string, PromptDefinition> dictionary = Service.Get<GameData>().Get<Dictionary<string, PromptDefinition>>();
			if (dictionary.ContainsKey(promptId))
			{
				PromptLoaderCMD promptLoaderCMD = new PromptLoaderCMD(this, dictionary[promptId], titleTextInsert, bodyTextInsert, showPrompt, callback);
				promptLoaderCMD.Execute();
			}
			else
			{
				Log.LogError(this, string.Format("Could not find prompt definition: {0}", promptId));
			}
		}

		public void ShowPrompt(PromptDefinition promptDefinition, Action<DPrompt.ButtonFlags> callback)
		{
			PromptLoaderCMD promptLoaderCMD = new PromptLoaderCMD(this, promptDefinition, showPrompt, callback);
			promptLoaderCMD.Execute();
		}

		private void showPrompt(PromptLoaderCMD promptLoader)
		{
			GameObject gameObject = ShowPrompt(promptLoader.PromptData, promptLoader.PromptCallback, promptLoader.Prefab);
			gameObject.name = promptLoader.PromptDefinition.Id;
		}

		private bool onSessionPaused(SessionEvents.SessionPausedEvent evt)
		{
			destroyAllPrompts();
			return false;
		}

		private void destroyAllPrompts()
		{
			for (int num = base.transform.childCount - 1; num >= 0; num--)
			{
				UnityEngine.Object.Destroy(base.transform.GetChild(num).gameObject);
			}
		}
	}
}
