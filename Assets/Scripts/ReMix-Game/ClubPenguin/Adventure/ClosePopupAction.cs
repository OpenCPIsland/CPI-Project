using ClubPenguin.UI;
using HutongGames.PlayMaker;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	[ActionCategory("GUI")]
	public class ClosePopupAction : FsmStateAction
	{
		public string PopupName;

		public bool CloseImmediate = false;

		public bool WaitForPopupComplete = true;

		private bool waitingForClose = false;

		private AnimatedPopup animatedPopup;

		public override void OnEnter()
		{
			GameObject gameObject = GameObject.Find(PopupName);
			if (gameObject == null)
			{
				gameObject = GameObject.Find(PopupName + "(Clone)");
			}
			if (gameObject != null)
			{
				animatedPopup = gameObject.GetComponent<AnimatedPopup>();
				if (animatedPopup != null)
				{
					animatedPopup.ClosePopup(CloseImmediate);
					if (WaitForPopupComplete)
					{
						animatedPopup.DoneClose += onDoneClose;
						waitingForClose = true;
					}
				}
				else
				{
					Object.Destroy(gameObject);
				}
			}
			if (!waitingForClose)
			{
				Finish();
			}
		}

		private void onDoneClose()
		{
			animatedPopup.DoneClose -= onDoneClose;
			Finish();
		}
	}
}
