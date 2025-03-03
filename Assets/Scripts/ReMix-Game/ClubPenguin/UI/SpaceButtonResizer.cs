using System;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.UI
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(OpenCloseTweener))]
	public class SpaceButtonResizer : MonoBehaviour
	{
		[DisallowMultipleComponent]
		private class GameObjectActiveListener : MonoBehaviour
		{
			public event Action<bool> OnGameObjectEnabledChanged;

			private void OnEnable()
			{
				if (this.OnGameObjectEnabledChanged != null)
				{
					this.OnGameObjectEnabledChanged(true);
				}
			}

			private void OnDisable()
			{
				if (this.OnGameObjectEnabledChanged != null)
				{
					this.OnGameObjectEnabledChanged(false);
				}
			}
		}

		[SerializeField]
		private GameObject cancelButton = null;

		private OpenCloseTweener openCloseTweener;

		private GameObjectActiveListener enabledListener;

		private bool tweenerInitialized;

		private bool cancelButtonEnabled = true;

		private void OnValidate()
		{
		}

		private void Awake()
		{
			openCloseTweener = GetComponent<OpenCloseTweener>();
			enabledListener = cancelButton.AddComponent<GameObjectActiveListener>();
		}

		public void OnEnable()
		{
			openCloseTweener.OnPositionChanged += onPositionChanged;
			enabledListener.OnGameObjectEnabledChanged += onGameObjectEnabledChanged;
		}

		public void OnDisable()
		{
			openCloseTweener.OnPositionChanged -= onPositionChanged;
			enabledListener.OnGameObjectEnabledChanged -= onGameObjectEnabledChanged;
		}

		private IEnumerator Start()
		{
			yield return null;
			openCloseTweener.Init(0f, ((RectTransform)cancelButton.transform).rect.width);
			tweenerInitialized = true;
			if (cancelButtonEnabled)
			{
				openCloseTweener.SetOpen();
			}
			else
			{
				openCloseTweener.SetClosed();
			}
		}

		private void onGameObjectEnabledChanged(bool subjectEnabled)
		{
			cancelButtonEnabled = subjectEnabled;
			if (tweenerInitialized)
			{
				if (cancelButtonEnabled)
				{
					openCloseTweener.Open();
				}
				else
				{
					openCloseTweener.Close();
				}
			}
		}

		private void onPositionChanged(float value)
		{
			((RectTransform)base.transform).offsetMin = new Vector2(value, ((RectTransform)base.transform).offsetMin.y);
		}
	}
}
