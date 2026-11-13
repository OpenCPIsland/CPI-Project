using ClubPenguin.Locomotion;
using ClubPenguin.World.Activities.Diving;
using Disney.LaunchPadFramework;
using UnityEngine;

public class DivingTrigger : MonoBehaviour
{
	public SwimControllerData MasterData;

	private void Awake()
	{
		if (MasterData == null)
		{
			Log.LogError(this, string.Format("Missing link to SwimControllerData class, some Diving sound effects will not work."));
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player") || other.CompareTag("RemotePlayer"))
		{
			DivingGameObserver divingGameObserver = other.gameObject.GetComponent<DivingGameObserver>();
			if (divingGameObserver == null)
			{
				divingGameObserver = other.gameObject.AddComponent<DivingGameObserver>();
			}
			divingGameObserver.DivingTriggerEnter(MasterData);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player") || other.CompareTag("RemotePlayer"))
		{
			DivingGameObserver component = other.gameObject.GetComponent<DivingGameObserver>();
			if ((bool)component)
			{
				component.DivingTriggerExit();
			}
		}
	}
}
