using ClubPenguin.IslandTargets;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

public class ClockTowerNotifier : MonoBehaviour
{
	public void OnEnable()
	{
		Service.Get<EventDispatcher>().DispatchEvent(new IslandTargetsEvents.ClockTowerStateChanged(true));
	}

	public void OnDisable()
	{
		Service.Get<EventDispatcher>().DispatchEvent(new IslandTargetsEvents.ClockTowerStateChanged(false));
	}
}
