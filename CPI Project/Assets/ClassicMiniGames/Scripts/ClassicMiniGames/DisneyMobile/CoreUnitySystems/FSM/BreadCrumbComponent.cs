using DisneyMobile.CoreUnitySystems.Utility;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace DisneyMobile.CoreUnitySystems.FSM
{
	public class BreadCrumbComponent : StateTraverserComponent
	{
		public struct Crumb
		{
			public State mStartState;

			public State mEndState;

			public Signal mSignal;

			public float mTime;

			public Crumb(State startState, State endState, Signal signal)
			{
				mStartState = startState;
				mEndState = endState;
				mSignal = signal;
				mTime = Time.realtimeSinceStartup;
			}
		}

		public Rect startRect = new Rect(10f, 10f, 375f, 200f);

		private List<Crumb> mTransitionHistory;

		private StateTraverser mTraverser = null;

		private GUIStyle mStyle;

		public Vector2 scrollPosition = Vector2.zero;

		public List<Crumb> TransitionHistory
		{
			get
			{
				return mTransitionHistory;
			}
		}

		public void Start()
		{
			mTraverser = base.gameObject.GetSafeComponent<StateTraverser>();
			if (mTraverser != null)
			{
				mTransitionHistory = new List<Crumb>();
				mTraverser.EventDispatcher.AddListener<StateTraverserTransitionBeganEvent>(OnTransitionBegan);
			}
		}

		public void Clear()
		{
			mTransitionHistory.Clear();
		}

		public bool OnTransitionBegan(StateTraverserTransitionBeganEvent evnt)
		{
			if (mTransitionHistory != null)
			{
				mTransitionHistory.Add(new Crumb(evnt.mPreviousState, evnt.mNewState, evnt.mSignal));
			}
			return false;
		}

		public override string ToString()
		{
			if (mTransitionHistory != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < mTransitionHistory.Count; i++)
				{
					Crumb crumb = mTransitionHistory[i];
					Transition obj = null;
					if (crumb.mSignal != null)
					{
						obj = crumb.mSignal.Transition;
					}
					stringBuilder.Append(string.Format("[{0}sec] state traverser '{1}' transitioning '{2}'->'{3}' using signal '{4}' and transition '{5}'\n", crumb.mTime, mTraverser.GetSafeName(), crumb.mStartState.GetSafeName(), crumb.mEndState.GetSafeName(), crumb.mSignal.GetSafeName(), obj.GetSafeName()));
				}
				return stringBuilder.ToString();
			}
			return string.Empty;
		}

		public void OnGUI()
		{
			if (mStyle == null)
			{
				mStyle = new GUIStyle(GUI.skin.label);
				mStyle.normal.textColor = Color.white;
				mStyle.alignment = TextAnchor.MiddleCenter;
			}
			startRect = GUI.Window(GUIUtility.GetControlID(FocusType.Passive), startRect, RenderWindow, "");
		}

		public void RenderWindow(int windowID)
		{
			GUILayout.BeginArea(startRect);
			scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(startRect.width - 100f), GUILayout.Height(startRect.height - 5f));
			GUI.skin.box.wordWrap = true;
			GUILayout.Label(ToString());
			GUI.EndScrollView();
			GUILayout.EndArea();
			GUI.DragWindow(new Rect(0f, 0f, Screen.width, Screen.height));
		}
	}
}
