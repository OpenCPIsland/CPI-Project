using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using HutongGames.PlayMaker;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Quest")]
	public class WaitForObjectToExistAction : FsmStateAction
	{
		[RequiredField]
		public FsmString ObjectName;

		public float TimeoutTime = 5f;

		public float CheckInterval = 0.2f;

		public bool HasTimeout = true;

		public FsmGameObject OUT_Object;

		private Timer timer;

		private float timeAlive = 0f;

		public override void OnEnter()
		{
			GameObject gameObject = GameObject.Find(ObjectName.Value);
			if (gameObject != null)
			{
				OUT_Object.Value = gameObject;
				Finish();
			}
			else
			{
				timer = new Timer(CheckInterval, true, delegate
				{
					onTimerTick();
				});
				CoroutineRunner.Start(timer.Start(), this, "WaitForObjectTimer");
			}
		}

		private void onTimerTick()
		{
			GameObject gameObject = GameObject.Find(ObjectName.Value);
			timeAlive += CheckInterval;
			if (gameObject != null)
			{
				OUT_Object.Value = gameObject;
				timer.Stop();
				CoroutineRunner.StopAllForOwner(this);
				Finish();
			}
			else if (HasTimeout && timeAlive > TimeoutTime)
			{
				timer.Stop();
				CoroutineRunner.StopAllForOwner(this);
				Disney.LaunchPadFramework.Log.LogErrorFormatted(this, "Object not found: {0}", ObjectName.Value);
				Finish();
			}
		}
	}
}
