using ClubPenguin.Analytics;
using ClubPenguin.Net;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections.Generic;

namespace ClubPenguin
{
	public class BackButtonStateHandler : AbstractAccountStateHandler
	{
		private class BackState
		{
			public string Name;

			public bool Valid;

			public BackState(string name, bool valid)
			{
				Name = name;
				Valid = valid;
			}
		}

		public string LastStateEvent;

		public string[] IgnoreStates = new string[2]
		{
			"background",
			"check"
		};

		private Stack<BackState> LastStates;

		public event Action<string, string> OnBackStateTransition;

		public new void Start()
		{
			base.Start();
			Service.Get<EventDispatcher>().AddListener<SessionEvents.SessionStartedEvent>(onSessionStarted);
		}

		public void onDestroy()
		{
			Service.Get<EventDispatcher>().RemoveListener<SessionEvents.SessionStartedEvent>(onSessionStarted);
		}

		public void OnStateChanged(string state)
		{
			if (LastStates == null)
			{
				LastStates = new Stack<BackState>();
				if (!string.IsNullOrEmpty(LastStateEvent))
				{
					LastStates.Push(new BackState(LastStateEvent, true));
				}
			}
			if (state == HandledState && rootStateMachine != null)
			{
				BackState backState = new BackState(string.Empty, false);
				BackState backState2 = new BackState("unknown", false);
				if (LastStates.Count > 0)
				{
					backState2 = LastStates.Pop();
				}
				bool flag = false;
				while (!flag && LastStates.Count > 0)
				{
					backState = LastStates.Pop();
					if (isStateValid(backState))
					{
						flag = true;
					}
				}
				if (!flag)
				{
					backState = new BackState(LastStateEvent, true);
				}
				Service.Get<ICPSwrveService>().NavigationAction("account_statemachine.back_button", backState2.Name, backState.Name);
				this.OnBackStateTransition.InvokeSafe(backState2.Name, backState.Name);
				rootStateMachine.SendEvent(backState.Name);
			}
			else
			{
				string text = state.ToLower();
				if (LastStates.Count == 0 || (text != LastStates.Peek().Name && !containsState(text)))
				{
					LastStates.Push(new BackState(text, !isStateIgnored(text)));
				}
			}
		}

		private bool containsState(string stateName)
		{
			foreach (BackState lastState in LastStates)
			{
				if (lastState.Name == stateName)
				{
					return true;
				}
			}
			return false;
		}

		public bool CanGoBack()
		{
			foreach (BackState lastState in LastStates)
			{
				if (isStateValid(lastState))
				{
					return true;
				}
			}
			return false;
		}

		private bool isStateValid(BackState state)
		{
			return state.Valid && rootStateMachine.CurrentStateName.ToLower() != state.Name;
		}

		public void MarkCurrentStateInvalid()
		{
			LastStates.Peek().Valid = false;
		}

		private bool isStateIgnored(string stateName)
		{
			int num = IgnoreStates.Length;
			for (int i = 0; i < num; i++)
			{
				if (stateName.IndexOf(IgnoreStates[i]) >= 0)
				{
					return true;
				}
			}
			return false;
		}

		private bool onSessionStarted(SessionEvents.SessionStartedEvent evt)
		{
			while (LastStates.Peek().Name == "createsuccess" || LastStates.Peek().Name == "createform" || LastStates.Peek().Name == "loginform" || LastStates.Peek().Name == "remembermesingleform" || LastStates.Peek().Name == "remembermemultipleform")
			{
				LastStates.Pop();
			}
			return false;
		}
	}
}
