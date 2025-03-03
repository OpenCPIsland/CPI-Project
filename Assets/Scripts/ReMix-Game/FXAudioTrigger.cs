using Disney.Kelowna.Common;
using Fabric;
using System.Collections;
using UnityEngine;

public class FXAudioTrigger : MonoBehaviour
{
	public string AudioEvent = "";

	public float Delay = 0f;

	public bool LoopEvent = false;

	public float LengthOfLoopSeconds = 1f;

	public GameObject OverrideSource = null;

	private void Awake()
	{
		Delay = Mathf.Clamp(Delay, 0f, LengthOfLoopSeconds);
		CoroutineRunner.Start(FXAudioLoop(), this, "");
		if (OverrideSource == null)
		{
			OverrideSource = base.gameObject;
		}
	}

	private IEnumerator FXAudioLoop()
	{
		bool loopValue = true;
		while (loopValue)
		{
			yield return new WaitForSeconds(Delay);
			if (base.gameObject.activeInHierarchy)
			{
				EventManager.Instance.PostEvent(AudioEvent, EventAction.PlaySound, OverrideSource);
			}
			yield return new WaitForSeconds(LengthOfLoopSeconds - Delay);
			loopValue = LoopEvent;
		}
	}

	private void OnDestroy()
	{
		CoroutineRunner.StopAllForOwner(this);
	}
}
