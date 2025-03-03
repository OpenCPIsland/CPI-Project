using Fabric;
using UnityEngine;

public class IslandTargetDamageHandler : MonoBehaviour
{
	private const float TOTAL_ROTATION = 180f;

	private const float ROTATE_TIME = 0.2f;

	public GameObject LightOn = null;

	public GameObject LightOff = null;

	public string RotateSFXTrigger = "SFX/AO/CrateCo/Target/Lever";

	public bool PlayRotateSFX = true;

	private float startRotation;

	private bool isTargetComplete;

	public void Reset()
	{
		isTargetComplete = false;
		LightOff.SetActive(true);
		LightOn.SetActive(false);
	}

	public void SetLightRotationAndState(float damagePercent)
	{
		if (!isTargetComplete)
		{
			if (damagePercent >= 1f)
			{
				damagePercent = 1f;
				isTargetComplete = true;
			}
			float num = 180f + 180f * damagePercent;
			iTween.Stop(base.gameObject);
			iTween.RotateTo(base.gameObject, iTween.Hash("name", "rotateTarget", "z", num, "time", 0.2f, "easetype", iTween.EaseType.easeOutBounce, "oncomplete", "onRotateAnimComplete", "onompletetarget", base.gameObject));
			if (PlayRotateSFX)
			{
				EventManager.Instance.PostEvent(RotateSFXTrigger, EventAction.PlaySound, base.gameObject);
			}
		}
	}

	private void onRotateAnimComplete()
	{
		if (isTargetComplete)
		{
			LightOff.SetActive(false);
			LightOn.SetActive(true);
		}
	}
}
