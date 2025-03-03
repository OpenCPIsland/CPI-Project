using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;
using UnityEngine;

namespace ClubPenguin.UI
{
	[ActionCategory("Cinematography")]
	public class IrisInOutAction : FsmStateAction
	{
		public const string GAMEOBJECT_NAME = "IrisInOutEffect";

		private const string ANIM_TRIGGER_OUT = "IrisTransitionClose";

		private const string ANIM_TRIGGER_IN = "IrisTransitionOpen";

		private static PrefabContentKey contentKey = new PrefabContentKey("FX/UI/Prefabs/IrisTransitionContainer");

		public bool IrisOut = true;

		public float DestroyDelay = 0.5f;

		public override void OnEnter()
		{
			string name = string.Format("CameraSpacePopupCanvas/{0}", "IrisInOutEffect");
			GameObject gameObject = GameObject.Find(name);
			if (gameObject == null)
			{
				if (IrisOut)
				{
					Content.LoadAsync(onPrefabLoaded, contentKey);
				}
				return;
			}
			if (IrisOut)
			{
				gameObject.GetComponentInChildren<Animator>().SetTrigger("IrisTransitionClose");
			}
			else
			{
				gameObject.GetComponentInChildren<Animator>().SetTrigger("IrisTransitionOpen");
				Object.Destroy(gameObject, DestroyDelay);
			}
			Finish();
		}

		private void onPrefabLoaded(string path, GameObject prefab)
		{
			GameObject gameObject = Object.Instantiate(prefab);
			gameObject.name = "IrisInOutEffect";
			Service.Get<EventDispatcher>().DispatchEvent(new PopupEvents.ShowCameraSpacePopup(gameObject));
			gameObject.GetComponentInChildren<Animator>().SetTrigger("IrisTransitionClose");
			Finish();
		}
	}
}
