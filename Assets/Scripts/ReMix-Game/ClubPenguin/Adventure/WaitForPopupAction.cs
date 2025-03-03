using ClubPenguin.UI;
using HutongGames.PlayMaker;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	[ActionCategory("GUI")]
	public class WaitForPopupAction : FsmStateAction
	{
		public string PopupName;

		private GameObject popup;

		public override void OnEnter()
		{
			PopupManager popupManager = SceneRefs.PopupManager;
			Transform transform = popupManager.transform.Find(PopupName);
			if (transform == null)
			{
				throw new UnityException(string.Format("WaitForPopupAction: Could not find popup '{0}' in state '{1}' of FSM '{2}'", PopupName, base.State.Name, base.Fsm.Name));
			}
			popup = transform.gameObject;
		}

		public override void OnUpdate()
		{
			if (popup == null)
			{
				Finish();
			}
		}
	}
}
