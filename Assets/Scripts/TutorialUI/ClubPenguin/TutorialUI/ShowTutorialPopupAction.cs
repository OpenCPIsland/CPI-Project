using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;
using UnityEngine;

namespace ClubPenguin.TutorialUI
{
	[ActionCategory("Tutorial")]
	public class ShowTutorialPopupAction : FsmStateAction
	{
		public FsmString PopupID;

		public string PrefabKey;

		public bool UseTarget;

		public string TargetName;

		public FsmGameObject TargetObject;

		public Vector2 Offset;

		public FsmVector2 Position;

		public float Scale = 1f;

		public override void OnEnter()
		{
			Content.LoadAsync<GameObject>(PrefabKey, onPrefabLoaded);
		}

		private void onPrefabLoaded(string path, GameObject prefab)
		{
			GameObject popup = Object.Instantiate(prefab);
			if (UseTarget)
			{
				if (!string.IsNullOrEmpty(TargetName))
				{
					GameObject gameObject = GameObject.Find(TargetName);
					if (gameObject != null && gameObject.GetComponent<RectTransform>() != null)
					{
						Service.Get<EventDispatcher>().DispatchEvent(new TutorialUIEvents.ShowTutorialPopup(PopupID.Value, popup, gameObject.GetComponent<RectTransform>(), Offset, Scale));
					}
					else
					{
						Disney.LaunchPadFramework.Log.LogError(this, "Show tutorial popup action target not found: " + TargetName);
					}
				}
				else if (TargetObject.Value == null || !TargetObject.Value.GetComponent<RectTransform>())
				{
					Disney.LaunchPadFramework.Log.LogError(this, "Show tutorial popup action target is null");
				}
				else
				{
					Service.Get<EventDispatcher>().DispatchEvent(new TutorialUIEvents.ShowTutorialPopup(PopupID.Value, popup, TargetObject.Value.GetComponent<RectTransform>(), Offset, Scale));
				}
			}
			else
			{
				Service.Get<EventDispatcher>().DispatchEvent(new TutorialUIEvents.ShowTutorialPopupAtPosition(PopupID.Value, popup, Position.Value, Scale));
			}
			Finish();
		}
	}
}
