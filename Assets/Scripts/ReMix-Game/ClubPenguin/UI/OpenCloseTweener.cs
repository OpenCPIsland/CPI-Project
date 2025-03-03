using System;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class OpenCloseTweener : MonoBehaviour
	{
		public float OpenSeconds;

		public float CloseSeconds;

		public iTween.EaseType OpenEasing;

		public iTween.EaseType CloseEasing;

		protected float openPosition;

		protected float closedPosition;

		protected bool isInit;

		private float totalRange;

		private Hashtable openParams;

		private Hashtable closeParams;

		private float currentPosition;

		[HideInInspector]
		public bool IsTransitioning;

		private string openTweenNameTag = "open";

		private string closeTweenNameTag = "close";

		public bool IsOpen
		{
			get;
			private set;
		}

		public event Action OnComplete;

		public event Action<float> OnPositionChanged;

		protected virtual void setPosition(float value)
		{
			if (this.OnPositionChanged != null)
			{
				this.OnPositionChanged(value);
			}
		}

		public void Init(float openPosition, float closedPosition)
		{
			openTweenNameTag += Guid.NewGuid().ToString();
			closeTweenNameTag += Guid.NewGuid().ToString();
			this.openPosition = openPosition;
			this.closedPosition = closedPosition;
			totalRange = openPosition - closedPosition;
			openParams = createTweenHash(closedPosition, openPosition, OpenSeconds, OpenEasing, openTweenNameTag);
			closeParams = createTweenHash(openPosition, closedPosition, CloseSeconds, CloseEasing, closeTweenNameTag);
			isInit = true;
		}

		public void Open()
		{
			if (base.gameObject.IsDestroyed())
			{
				return;
			}
			if (!IsTransitioning)
			{
				IsTransitioning = true;
				if (IsOpen && Math.Abs(currentPosition - openPosition) > float.Epsilon)
				{
					Hashtable args = createTransitionHash(currentPosition, openPosition, totalRange, OpenSeconds, OpenEasing, openTweenNameTag);
					iTween.ValueTo(base.gameObject, args);
				}
				else
				{
					iTween.ValueTo(base.gameObject, openParams);
				}
			}
			else
			{
				Hashtable args = createTransitionHash(currentPosition, openPosition, totalRange, OpenSeconds, OpenEasing, openTweenNameTag);
				iTween.StopByName(closeTweenNameTag);
				iTween.ValueTo(base.gameObject, args);
			}
			IsOpen = true;
		}

		public void Close()
		{
			if (!IsTransitioning)
			{
				IsTransitioning = true;
				if (!IsOpen && Math.Abs(currentPosition - closedPosition) > float.Epsilon)
				{
					Hashtable args = createTransitionHash(currentPosition, closedPosition, totalRange, CloseSeconds, CloseEasing, closeTweenNameTag);
					iTween.ValueTo(base.gameObject, args);
				}
				else
				{
					iTween.ValueTo(base.gameObject, closeParams);
				}
			}
			else
			{
				Hashtable args = createTransitionHash(currentPosition, closedPosition, totalRange, CloseSeconds, CloseEasing, closeTweenNameTag);
				iTween.StopByName(openTweenNameTag);
				iTween.ValueTo(base.gameObject, args);
			}
			IsOpen = false;
		}

		public void SetOpen()
		{
			iTween.Stop(base.gameObject);
			updatePosition(openPosition);
			IsOpen = true;
			onComplete();
		}

		public void SetClosed()
		{
			iTween.Stop(base.gameObject);
			updatePosition(closedPosition);
			IsOpen = false;
			onComplete();
		}

		public bool Resize(float openPosition)
		{
			this.openPosition = openPosition;
			totalRange = openPosition - closedPosition;
			openParams = createTweenHash(closedPosition, openPosition, OpenSeconds, OpenEasing, openTweenNameTag);
			closeParams = createTweenHash(openPosition, closedPosition, CloseSeconds, CloseEasing, closeTweenNameTag);
			return false;
		}

		private Hashtable createTweenHash(float from, float to, float seconds, iTween.EaseType easing, string tweenName)
		{
			return iTween.Hash("from", from, "to", to, "time", seconds, "onupdate", "updatePosition", "oncomplete", "onComplete", "easetype", easing, "name", tweenName);
		}

		private Hashtable createTransitionHash(float from, float to, float range, float seconds, iTween.EaseType easing, string tweenName)
		{
			float num = to - from;
			float seconds2 = Mathf.Abs(num / range * seconds);
			return createTweenHash(from, to, seconds2, easing, tweenName);
		}

		private void updatePosition(float value)
		{
			currentPosition = value;
			setPosition(value);
		}

		private void onComplete()
		{
			IsTransitioning = false;
			if (this.OnComplete != null)
			{
				this.OnComplete();
			}
		}

		private void OnDestroy()
		{
			iTween.Stop(base.gameObject);
			this.OnPositionChanged = null;
		}
	}
}
