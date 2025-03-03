using ClubPenguin.Locomotion;
using ClubPenguin.SledRacer;
using UnityEngine;

namespace ClubPenguin.Actions
{
	public class SetRaceLocomotionAction : Action
	{
		public GameObject RaceGate;

		public GameObject Lite1a;

		public GameObject Lite2a;

		public GameObject Lite3a;

		public GameObject Lite1b;

		public GameObject Lite2b;

		public GameObject Lite3b;

		public long SilverTimeMS = 15000L;

		public long GoldTimeMS = 10000L;

		public long LegendaryTimeMS = 4000L;

		public string TrackId = "";

		protected override void CopyTo(Action _destWarper)
		{
			SetRaceLocomotionAction setRaceLocomotionAction = _destWarper as SetRaceLocomotionAction;
			setRaceLocomotionAction.RaceGate = RaceGate;
			setRaceLocomotionAction.Lite1a = Lite1a;
			setRaceLocomotionAction.Lite2a = Lite2a;
			setRaceLocomotionAction.Lite3a = Lite3a;
			setRaceLocomotionAction.Lite1b = Lite1b;
			setRaceLocomotionAction.Lite2b = Lite2b;
			setRaceLocomotionAction.Lite3b = Lite3b;
			setRaceLocomotionAction.SilverTimeMS = SilverTimeMS;
			setRaceLocomotionAction.GoldTimeMS = GoldTimeMS;
			setRaceLocomotionAction.LegendaryTimeMS = LegendaryTimeMS;
			setRaceLocomotionAction.TrackId = TrackId;
			base.CopyTo(_destWarper);
		}

		protected override void Update()
		{
			GameObject target = GetTarget();
			if (target == SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject && !LocomotionHelper.IsCurrentControllerOfType<RaceController>(target))
			{
				LocomotionHelper.SetCurrentController<RaceController>(target);
				LocomotionController currentController = LocomotionHelper.GetCurrentController(target);
				if (currentController is RaceController)
				{
					RaceGameController raceGameController = target.GetComponent<RaceGameController>();
					if (raceGameController == null)
					{
						raceGameController = target.AddComponent<RaceGameController>();
					}
					((RaceController)currentController).InitializeRace(raceGameController);
					raceGameController.InitializeRace(RaceGate, Lite1a, Lite2a, Lite3a, Lite1b, Lite2b, Lite3b, SilverTimeMS, GoldTimeMS, LegendaryTimeMS, TrackId);
				}
			}
			Completed();
		}
	}
}
