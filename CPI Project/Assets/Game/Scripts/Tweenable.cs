using System;
using UnityEngine;

public class Tweenable : MonoBehaviour
{
	public Action<GameObject> TweenCompleteAction;

	public void TweenPosition(Vector3 destinationPosition, float tweenTime, bool dispatchComplete = true)
	{
		if (dispatchComplete)
		{
			iTween.MoveTo(base.gameObject, iTween.Hash("position", destinationPosition, "time", tweenTime, "oncomplete", "OnTweenComplete", "oncompletetarget", base.gameObject));
		}
		else
		{
			iTween.MoveTo(base.gameObject, destinationPosition, tweenTime);
		}
	}

	public void TweenScale(Vector3 destinationScale, float tweenTime, bool dispatchComplete = false)
	{
		if (dispatchComplete)
		{
			iTween.ScaleTo(base.gameObject, iTween.Hash("scale", destinationScale, "time", tweenTime, "oncomplete", "OnTweenComplete", "oncompletetarget", base.gameObject));
		}
		else
		{
			iTween.ScaleTo(base.gameObject, destinationScale, tweenTime);
		}
	}

	public void OnTweenComplete()
	{
		if (TweenCompleteAction != null)
		{
			TweenCompleteAction(base.gameObject);
		}
	}
}
