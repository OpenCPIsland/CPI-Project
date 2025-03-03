using ClubPenguin.Adventure;
using ClubPenguin.Core;
using ClubPenguin.SledRacer;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.Locomotion
{
	public class RaceControllerTrigger : MonoBehaviour
	{
		public enum ActionType
		{
			StartingLine,
			FinishLine,
			EndOfTrack
		}

		public bool IsMemberOnly = true;

		[Tooltip("BeginingOfTrack disables on exit. EndOfTrack disables on enter.")]
		public ActionType Action = ActionType.FinishLine;

		public string Tag = "Player";

		private bool hasPopupOpened = false;

		private void Start()
		{
			Service.Get<EventDispatcher>().AddListener<RaceGameEvents.RaceFinishPopupOpened>(onRaceFinishPopupOpened);
		}

		private void OnDestroy()
		{
			Service.Get<EventDispatcher>().RemoveListener<RaceGameEvents.RaceFinishPopupOpened>(onRaceFinishPopupOpened);
		}

		public void OnTriggerEnter(Collider col)
		{
			if (!(col != null) || !col.CompareTag(Tag) || !(col.gameObject != null) || (IsMemberOnly && !Service.Get<CPDataEntityCollection>().IsLocalPlayerMember()))
			{
				return;
			}
			if (Action == ActionType.StartingLine)
			{
				RaceController component = col.gameObject.GetComponent<RaceController>();
				if ((object)component != null)
				{
					component.SetInitialTrackDir(base.transform.forward);
					hasPopupOpened = false;
				}
			}
			else if (Action == ActionType.FinishLine)
			{
				if (LocomotionHelper.IsCurrentControllerOfType<RaceController>(col.gameObject))
				{
					RaceGameController raceGameController = col.gameObject.GetComponent<RaceGameController>();
					if (raceGameController == null)
					{
						raceGameController = col.gameObject.AddComponent<RaceGameController>();
					}
					raceGameController.FinishRace();
				}
			}
			else if (Action == ActionType.EndOfTrack && LocomotionHelper.IsCurrentControllerOfType<RaceController>(col.gameObject))
			{
				RaceController component = col.gameObject.GetComponent<RaceController>();
				if (component != null)
				{
					component.setControlsEnabled(true);
				}
				LocomotionHelper.SetCurrentController<RunController>(col.gameObject);
				RaceGameController raceGameController = col.gameObject.GetComponent<RaceGameController>();
				if (raceGameController != null)
				{
					raceGameController.RemoveLocalPlayerRaceData();
					Object.Destroy(raceGameController);
				}
				if (!hasPopupOpened)
				{
					Service.Get<QuestService>().SendEvent("FinishredRace");
				}
			}
		}

		public void OnTriggerExit(Collider col)
		{
			if (col != null && col.CompareTag(Tag) && col.gameObject != null && !LocomotionHelper.IsCurrentControllerOfType<RaceController>(col.gameObject) && Action == ActionType.StartingLine)
			{
				RaceController component = col.gameObject.GetComponent<RaceController>();
				if (component != null)
				{
					component.setControlsEnabled(true);
				}
			}
		}

		private bool onRaceFinishPopupOpened(RaceGameEvents.RaceFinishPopupOpened evt)
		{
			hasPopupOpened = true;
			return false;
		}
	}
}
