using UnityEngine;

namespace ClubPenguin.Actions
{
	public class DestroyObjectAction : Action
	{
		public GameObject TheObject;

		public bool DestroyOnInterrupt;

		public float Delay;

		protected override void CopyTo(Action _destWarper)
		{
			DestroyObjectAction destroyObjectAction = _destWarper as DestroyObjectAction;
			destroyObjectAction.TheObject = TheObject;
			destroyObjectAction.DestroyOnInterrupt = DestroyOnInterrupt;
			destroyObjectAction.Delay = Delay;
			base.CopyTo(_destWarper);
		}

		public override void Completed(object userData = null, bool conditionBranchValue = true)
		{
			if (DestroyOnInterrupt)
			{
				doDestroy();
			}
			base.Completed(userData);
		}

		private void doDestroy()
		{
			float t = Mathf.Clamp(Delay, 0f, float.PositiveInfinity);
			if (TheObject != null)
			{
				Object.Destroy(TheObject, t);
			}
			else if (IncomingUserData != null && IncomingUserData is GameObject)
			{
				Object.Destroy((GameObject)IncomingUserData, t);
			}
		}

		protected override void Update()
		{
			if (!DestroyOnInterrupt)
			{
				doDestroy();
				Completed();
			}
		}
	}
}
