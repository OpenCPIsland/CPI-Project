using ClubPenguin.Core;
using ClubPenguin.Net;
using ClubPenguin.Net.Domain;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Disney.Native;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ClubPenguin.Tutorial
{
	public class TutorialManager
	{
		public struct TutorialStartedEvent
		{
			public int TutorialId;

			public TutorialStartedEvent(int id)
			{
				TutorialId = id;
			}
		}

		private const string TUTORIAL_FSM_CONTAINER_NAME = "TutorialFSMContainer";

		private const bool DEFAULT_TUTORIAL_VALUE = false;

		private const int PROGRESSION_TUTORIAL_ID = 0;

		public Action<TutorialDefinition> TutorialCompleteAction;

		public int PlayerAgeInDays;

		public bool Disabled;

		private Dictionary<int, TutorialDefinition> tutorialDefinitions;

		private TutorialDefinition currentTutorial;

		private CPDataEntityCollection dataEntityCollection;

		private ITutorialService tutorialService;

		private GameObject fsmContainer;

		private TutorialData tutorialData;

		public TutorialManager()
		{
			tutorialDefinitions = Service.Get<IGameData>().Get<Dictionary<int, TutorialDefinition>>();
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
		}

		public bool TryStartTutorial(TutorialDefinition tutorial, string sceneNameToRunTutorial = null)
		{
			bool result = false;
			if (!(currentTutorial != null) && IsTutorialAvailable(tutorial))
			{
				loadTutorial(tutorial, sceneNameToRunTutorial);
				result = true;
			}
			return result;
		}

		public bool TryStartTutorial(int tutorialID, string sceneNameToRunTutorial = null)
		{
			bool result = false;
			TutorialDefinition value;
			if (tutorialDefinitions.TryGetValue(tutorialID, out value))
			{
				result = TryStartTutorial(value, sceneNameToRunTutorial);
			}
			return result;
		}

		public void EndTutorial()
		{
			if (currentTutorial != null)
			{
				if (!currentTutorial.IsNotAutoComplete)
				{
					SetTutorial(currentTutorial.Id, true);
				}
				if (TutorialCompleteAction != null)
				{
					TutorialCompleteAction(currentTutorial);
				}
				currentTutorial = null;
				UnityEngine.Object.Destroy(getFSMContainer());
			}
		}

		public void SetTutorial(int tutorialID, bool isComplete)
		{
			if (getTutorialData() != null)
			{
				bool flag = false;
				if (tutorialID < getTutorialData().Data.Count && getTutorialData().Data.Get(tutorialID) != isComplete)
				{
					getTutorialData().Data.Set(tutorialID, isComplete);
					flag = true;
				}
				else if (tutorialID >= getTutorialData().Data.Count && isComplete)
				{
					getTutorialData().Data.Length = tutorialID + 1;
					getTutorialData().Data.Set(tutorialID, isComplete);
					flag = true;
				}
				if (flag)
				{
					ClubPenguin.Net.Domain.Tutorial tutorial = new ClubPenguin.Net.Domain.Tutorial(tutorialID, isComplete);
					Service.Get<EventDispatcher>().AddListener<TutorialServiceEvents.TutorialReceived>(onTutorialReceived);
					getTutorialService().SetTutorial(tutorial);
				}
			}
		}

		public bool IsTutorialRunning()
		{
			return currentTutorial != null;
		}

		public bool IsTutorialAvailable(int tutorialID)
		{
			bool result = false;
			TutorialDefinition value;
			if (tutorialDefinitions.TryGetValue(tutorialID, out value))
			{
				result = IsTutorialAvailable(value);
			}
			return result;
		}

		public bool IsTutorialAvailable(TutorialDefinition tutorial)
		{
			bool flag = true;
			flag = !isTutorialComplete(tutorial.Id);
			if (flag && MonoSingleton<NativeAccessibilityManager>.Instance.AccessibilityLevel != 0)
			{
				flag = false;
			}
			if (flag && Disabled && tutorial.Id != 0)
			{
				flag = false;
			}
			if (flag && tutorial.IsMemberOnly)
			{
				flag = dataEntityCollection.IsLocalPlayerMember();
			}
			if (flag && tutorial.MinimumPenguinAge > 0 && PlayerAgeInDays < tutorial.MinimumPenguinAge)
			{
				flag = false;
			}
			if (flag)
			{
				flag = isTutorialRequirementsComplete(tutorial);
			}
			return flag;
		}

		private void loadTutorial(TutorialDefinition tutorial, string sceneNameToRunTutorial)
		{
			if (tutorial.FsmContentKey != null && !string.IsNullOrEmpty(tutorial.FsmContentKey.Key) && Content.ContainsKey(tutorial.FsmContentKey.Key))
			{
				currentTutorial = tutorial;
				Content.LoadAsync(delegate(string path, FsmTemplate fsm)
				{
					onLoadFSMComplete(fsm, sceneNameToRunTutorial);
				}, tutorial.FsmContentKey);
			}
		}

		private void onLoadFSMComplete(FsmTemplate fsm, string sceneNameToRunTutorial)
		{
			startTutorialFSM(fsm, sceneNameToRunTutorial);
		}

		private void startTutorialFSM(FsmTemplate fsm, string sceneNameToRunTutorial)
		{
			Service.Get<EventDispatcher>().DispatchEvent(new TutorialStartedEvent(currentTutorial.Id));
			PlayMakerFSM playMakerFSM = getFSMContainer().AddComponent<PlayMakerFSM>();
			playMakerFSM.Fsm.Name = currentTutorial.name;
			playMakerFSM.SetFsmTemplate(fsm);
			if (!string.IsNullOrEmpty(sceneNameToRunTutorial))
			{
				SceneManager.MoveGameObjectToScene(fsmContainer, SceneManager.GetSceneByName(sceneNameToRunTutorial));
			}
		}

		private bool isTutorialRequirementsComplete(TutorialDefinition tutorial)
		{
			bool result = true;
			for (int i = 0; i < tutorial.TutorialRequirements.Length; i++)
			{
				if (!isTutorialComplete(tutorial.TutorialRequirements[i].Id))
				{
					result = false;
					break;
				}
			}
			return result;
		}

		private bool isTutorialComplete(int tutorialID)
		{
			if (getTutorialData() == null)
			{
				return true;
			}
			bool result = false;
			if (getTutorialData().Data.Count > tutorialID)
			{
				result = getTutorialData().Data.Get(tutorialID);
			}
			return result;
		}

		private bool onTutorialReceived(TutorialServiceEvents.TutorialReceived evt)
		{
			Service.Get<EventDispatcher>().RemoveListener<TutorialServiceEvents.TutorialReceived>(onTutorialReceived);
			if (evt.TutorialBytes != null && evt.TutorialBytes.Length > 0)
			{
				getTutorialData().Data = new BitArray(evt.TutorialBytes);
			}
			return false;
		}

		private Dictionary<int, TutorialDefinition> parseTutorialManifest(Manifest manifest)
		{
			Dictionary<int, TutorialDefinition> dictionary = new Dictionary<int, TutorialDefinition>();
			for (int i = 0; i < manifest.Assets.Length; i++)
			{
				TutorialDefinition tutorialDefinition = manifest.Assets[i] as TutorialDefinition;
				dictionary.Add(tutorialDefinition.Id, tutorialDefinition);
			}
			return dictionary;
		}

		private ITutorialService getTutorialService()
		{
			if (tutorialService == null)
			{
				tutorialService = Service.Get<INetworkServicesManager>().TutorialService;
			}
			return tutorialService;
		}

		private GameObject getFSMContainer()
		{
			if (fsmContainer == null)
			{
				fsmContainer = new GameObject("TutorialFSMContainer");
			}
			return fsmContainer;
		}

		private TutorialData getTutorialData()
		{
			if (tutorialData == null)
			{
				tutorialData = dataEntityCollection.GetComponent<TutorialData>(dataEntityCollection.LocalPlayerHandle);
			}
			return tutorialData;
		}
	}
}
