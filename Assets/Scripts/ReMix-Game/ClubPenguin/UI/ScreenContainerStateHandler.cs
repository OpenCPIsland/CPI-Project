using ClubPenguin.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections;
using Tweaker.Core;
using UnityEngine;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(LayoutElementHeightTweener))]
	public class ScreenContainerStateHandler : MonoBehaviour, IScreenContainerStateHandler
	{
		private enum ScreenContainerState
		{
			Min,
			Max
		}

		private ScreenContainerState currentState;

		private EventChannel eventChannel;

		private LayoutElementHeightTweener layoutElementTweener;

		private RectTransform canvasRectTransform;

		private float screenHeight = -1f;

		private GameObject containerBG;

		private bool isOpen;

		public float DEFAULT_HEIGHT = 0.34f;

		[Tweakable("UI.ScreenContainer.ShouldAnimate")]
		public static bool ShouldAnimate = false;

		public bool ShowScreenContainerBG
		{
			get;
			private set;
		}

		public bool IsKeyboardShown
		{
			get;
			private set;
		}

		public bool IsOpen
		{
			get
			{
				return isOpen;
			}
		}

		public void Awake()
		{
			eventChannel = new EventChannel(Service.Get<EventDispatcher>());
			setSceneRef();
		}

		public void Start()
		{
			layoutElementTweener = GetComponent<LayoutElementHeightTweener>();
			DEFAULT_HEIGHT *= GetComponentInParent<CanvasScalerExt>().ScaleModifier;
			containerBG = GetComponentInChildren<ScreenContainerBG>(true).gameObject;
			RectTransform content = base.transform.GetChild(0) as RectTransform;
			layoutElementTweener.Content = content;
			canvasRectTransform = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
			CoroutineRunner.Start(waitForLayout(), this, "waitForLayout");
			layoutElementTweener.OnComplete += onLayoutTweenerComplete;
			IsKeyboardShown = false;
			isOpen = false;
		}

		public void OnEnable()
		{
			addListeners();
			setSceneRef();
		}

		public void OnDisable()
		{
			removeListeners();
			removeSceneRef();
		}

		public void OnDestroy()
		{
			removeSceneRef();
		}

		private void setSceneRef()
		{
			if (ClubPenguin.Core.SceneRefs.IsSet<IScreenContainerStateHandler>())
			{
				IScreenContainerStateHandler screenContainerStateHandler = ClubPenguin.Core.SceneRefs.Get<IScreenContainerStateHandler>();
				if (!object.ReferenceEquals(screenContainerStateHandler, this))
				{
					ClubPenguin.Core.SceneRefs.Remove(screenContainerStateHandler);
					ClubPenguin.Core.SceneRefs.Set((IScreenContainerStateHandler)this);
				}
			}
			else
			{
				ClubPenguin.Core.SceneRefs.Set((IScreenContainerStateHandler)this);
			}
		}

		private void removeSceneRef()
		{
			if (!ClubPenguin.Core.SceneRefs.IsSet<IScreenContainerStateHandler>())
			{
				ClubPenguin.Core.SceneRefs.Remove((IScreenContainerStateHandler)this);
			}
		}

		private IEnumerator waitForLayout()
		{
			while (Math.Abs(canvasRectTransform.rect.height) < float.Epsilon)
			{
				yield return null;
			}
			layoutElementTweener.Init(canvasRectTransform.rect.height * DEFAULT_HEIGHT, 0f);
		}

		public void OnStateChanged(string newStateString)
		{
			ScreenContainerState screenContainerState = (ScreenContainerState)Enum.Parse(typeof(ScreenContainerState), newStateString);
			switch (screenContainerState)
			{
			case ScreenContainerState.Min:
				layoutElementTweener.Close();
				break;
			case ScreenContainerState.Max:
				layoutElementTweener.Open();
				ShowScreenContainerBG = true;
				containerBG.SetActive(true);
				break;
			}
			currentState = screenContainerState;
		}

		private void onLayoutTweenerComplete()
		{
			switch (currentState)
			{
			case ScreenContainerState.Min:
				ShowScreenContainerBG = false;
				containerBG.SetActive(false);
				Service.Get<EventDispatcher>().DispatchEvent(default(TrayEvents.TrayClosed));
				isOpen = false;
				break;
			case ScreenContainerState.Max:
				Service.Get<EventDispatcher>().DispatchEvent(default(TrayEvents.TrayOpened));
				isOpen = true;
				break;
			}
		}

		private void addListeners()
		{
			eventChannel.AddListener<KeyboardEvents.KeyboardShown>(onKeyboardShown, EventDispatcher.Priority.HIGH);
			eventChannel.AddListener<KeyboardEvents.KeyboardHidden>(onKeyboardHidden, EventDispatcher.Priority.HIGH);
		}

		private void removeListeners()
		{
			eventChannel.RemoveAllListeners();
		}

		private void setScreenContainerHeight(float normalizedHeight)
		{
			layoutElementTweener.Resize(canvasRectTransform.rect.height * normalizedHeight);
		}

		private bool onKeyboardShown(KeyboardEvents.KeyboardShown evt)
		{
			IsKeyboardShown = true;
			if (evt.Height > 0)
			{
				float screenContainerHeight = (float)evt.Height / getScreenHeight();
				setScreenContainerHeight(screenContainerHeight);
				if (ShouldAnimate)
				{
					layoutElementTweener.Open();
				}
				else
				{
					layoutElementTweener.SetOpen();
				}
			}
			return false;
		}

		private float getScreenHeight()
		{
			if (screenHeight < 0f)
			{
				AccessibilityManager accessibilityManager = Service.Get<AccessibilityManager>();
				screenHeight = accessibilityManager.GetScreenSizeWithSoftKeys().y;
			}
			return screenHeight;
		}

		private bool onKeyboardHidden(KeyboardEvents.KeyboardHidden evt)
		{
			if (IsKeyboardShown)
			{
				IsKeyboardShown = false;
				setScreenContainerHeight(DEFAULT_HEIGHT);
				switch (currentState)
				{
				case ScreenContainerState.Max:
					layoutElementTweener.SetOpen();
					break;
				case ScreenContainerState.Min:
					layoutElementTweener.SetClosed();
					break;
				}
			}
			return false;
		}
	}
}
