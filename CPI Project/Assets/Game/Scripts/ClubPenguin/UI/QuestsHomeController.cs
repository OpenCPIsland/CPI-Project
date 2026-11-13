using ClubPenguin.Adventure;
using ClubPenguin.Analytics;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.SEDFSM;
using Disney.MobileNetwork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class QuestsHomeController : MonoBehaviour
	{
		public Transform ScrollContent;

		public bool UnloadAllObjectsOnUnload;

		private IList<string> mascotKeys;

		private PrefabContentKey questLogMascotItemContentKey = new PrefabContentKey("Prefabs/Quest/QuestMascotItems/QuestMascotItem_*");

		public void Start()
		{
			mascotKeys = new List<string>();
			loadQuestMascotItems(Service.Get<MascotService>().Mascots);
		}

		private void loadQuestMascotItems(IEnumerable<Mascot> mascotData)
		{
			List<Mascot> list = new List<Mascot>();
			foreach (Mascot mascotDatum in mascotData)
			{
				if (mascotDatum.IsQuestGiver || mascotDatum.Definition.ShowComingSoonInLog)
				{
					list.Add(mascotDatum);
				}
			}
			list.Sort((Mascot a, Mascot b) => a.Definition.QuestLogPriority.CompareTo(b.Definition.QuestLogPriority));
			CoroutineRunner.Start(loadQuestMascotItems(list), this, "loadQuestMascotItems");
		}

		private IEnumerator loadQuestMascotItems(List<Mascot> mascots)
		{
			for (int i = 0; i < mascots.Count; i++)
			{
				yield return loadQuestMascotItem(mascots[i]);
			}
		}

		private IEnumerator loadQuestMascotItem(Mascot mascot)
		{
			PrefabContentKey mascotItemContentKey = new PrefabContentKey(questLogMascotItemContentKey, mascot.Name);
			AssetRequest<GameObject> assetRequest = Content.LoadAsync(mascotItemContentKey);
			yield return assetRequest;
			mascotKeys.Add(mascotItemContentKey.Key);
			GameObject mascotItemGameObject = Object.Instantiate(assetRequest.Asset);
			Button button = mascotItemGameObject.GetComponentInChildren<Button>();
			if (button != null)
			{
				button.onClick.AddListener(delegate
				{
					onMascotButtonPressed(mascot.Name);
				});
			}
			QuestLogMascotItem mascotItem = mascotItemGameObject.GetComponent<QuestLogMascotItem>();
			mascotItem.LoadMascotData(mascot);
			mascotItemGameObject.transform.SetParent(ScrollContent, false);
		}

		private void onMascotButtonPressed(string mascotName)
		{
			QuestsScreenController questsScreenController = GetComponentInParent<QuestsScreenController>();
			if (questsScreenController == null)
			{
				GameObject gameObject = GetComponentInParent<StateMachineContext>().gameObject;
				questsScreenController = gameObject.GetComponentInChildren<QuestsScreenController>();
			}
			questsScreenController.CurrentMascotID = mascotName;
			Service.Get<ICPSwrveService>().Action("game.quest", "mascot", mascotName);
		}

		public void OnDisable()
		{
			for (int i = 0; i < mascotKeys.Count; i++)
			{
				Content.Unload<GameObject>(mascotKeys[i], UnloadAllObjectsOnUnload);
			}
		}
	}
}
