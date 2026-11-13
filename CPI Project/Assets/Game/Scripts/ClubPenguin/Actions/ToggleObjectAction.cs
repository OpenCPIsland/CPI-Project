using UnityEngine;

namespace ClubPenguin.Actions
{
	public class ToggleObjectAction : Action
	{
		public GameObject TheObject;

		public bool Toggle;

		public bool On;

		public bool ToggleOnInterrupt;

		public bool ForLocalPlayerOnly;

		protected override void CopyTo(Action _destWarper)
		{
			ToggleObjectAction toggleObjectAction = _destWarper as ToggleObjectAction;
			toggleObjectAction.TheObject = TheObject;
			toggleObjectAction.On = On;
			toggleObjectAction.Toggle = Toggle;
			toggleObjectAction.ToggleOnInterrupt = ToggleOnInterrupt;
			toggleObjectAction.ForLocalPlayerOnly = ForLocalPlayerOnly;
			base.CopyTo(_destWarper);
		}

		public override void Completed(object userData = null, bool conditionBranchValue = true)
		{
			if (ToggleOnInterrupt)
			{
				doToggle();
			}
			base.Completed(userData);
		}

		private void doToggle()
		{
			if (ForLocalPlayerOnly && !base.gameObject.CompareTag("Player"))
			{
				return;
			}
			GameObject gameObject = TheObject;
			if (gameObject == null && IncomingUserData != null && IncomingUserData is GameObject)
			{
				gameObject = (GameObject)IncomingUserData;
			}
			if (gameObject != null)
			{
				if (Toggle)
				{
					gameObject.SetActive(!gameObject.activeInHierarchy);
				}
				else
				{
					gameObject.SetActive(On);
				}
			}
		}

		protected override void Update()
		{
			if (!ToggleOnInterrupt)
			{
				doToggle();
				Completed();
			}
		}
	}
}
