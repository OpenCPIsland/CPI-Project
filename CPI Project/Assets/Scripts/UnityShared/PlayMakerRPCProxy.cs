using UnityEngine;

public class PlayMakerRPCProxy : MonoBehaviour
{
	public PlayMakerFSM[] fsms;

	public void Reset()
	{
		fsms = GetComponents<PlayMakerFSM>();
	}

	public void ForwardEvent(string eventName)
	{
		PlayMakerFSM[] array = fsms;
		foreach (PlayMakerFSM playMakerFSM in array)
		{
			playMakerFSM.SendEvent(eventName);
		}
	}
}
