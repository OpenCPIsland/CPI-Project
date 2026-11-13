using DisneyMobile.CoreUnitySystems.Utility;
using UnityEngine;

namespace DisneyMobile.CoreUnitySystems.FSM
{
	public class CurrentStateHUD : MonoBehaviour
	{
		public StateTraverser stateTraverser;

		public Rect startRect = new Rect(10f, 10f, 375f, 30f);

		public bool allowDrag = true;

		public bool forceToLowerLeft = true;

		public float offsetFromBottomWhenForcedToLowerLeft = 0f;

		public Color mColor = Color.white;

		private GUIStyle mStyle;

		private string mLabel = "No Traverser Specified";

		public void Start()
		{
			if (forceToLowerLeft)
			{
				Rect rect = startRect = new Rect(0f, (float)Screen.height - startRect.height - offsetFromBottomWhenForcedToLowerLeft, startRect.width, startRect.height);
			}
			if (stateTraverser != null)
			{
				stateTraverser.EventDispatcher.AddListener<StateTraverserStateChangeEvent>(OnStateChange);
				stateTraverser.EventDispatcher.AddListener<StateTraverserTransitionBeganEvent>(OnTransitionBegan);
				mLabel = GenerateLabel(null, stateTraverser.CurrentState, null, false);
			}
		}

		public bool OnStateChange(StateTraverserStateChangeEvent evnt)
		{
			mLabel = GenerateLabel(null, evnt.mNewState, null, false);
			return false;
		}

		public bool OnTransitionBegan(StateTraverserTransitionBeganEvent evnt)
		{
			mLabel = GenerateLabel(evnt.mPreviousState, evnt.mNewState, evnt.mSignal, true);
			return false;
		}

		public void OnGUI()
		{
			if (stateTraverser.gameObject.activeInHierarchy && stateTraverser.CurrentState != null)
			{
				if (mStyle == null)
				{
					mStyle = new GUIStyle(GUI.skin.label);
					mStyle.normal.textColor = Color.white;
					mStyle.alignment = TextAnchor.MiddleCenter;
				}
				GUI.color = mColor;
				startRect = GUI.Window(GUIUtility.GetControlID(FocusType.Passive), startRect, RenderWindow, "");
			}
		}

		public void RenderWindow(int windowID)
		{
			Rect position = new Rect(0f, 0f, startRect.width, startRect.height);
			GUI.Label(position, mLabel, mStyle);
			if (allowDrag)
			{
				GUI.DragWindow(new Rect(0f, 0f, Screen.width, Screen.height));
			}
		}

		private string GenerateLabel(State previousState, State newState, Signal signal, bool transitioning)
		{
			if (transitioning)
			{
				return string.Format("{0} : {1}->{2} (({3}))", stateTraverser.GetSafeName(), previousState.GetSafeName(), newState.GetSafeName(), signal.GetSafeName());
			}
			return string.Format("{0} : {1}", stateTraverser.GetSafeName(), newState.GetSafeName());
		}
	}
}
