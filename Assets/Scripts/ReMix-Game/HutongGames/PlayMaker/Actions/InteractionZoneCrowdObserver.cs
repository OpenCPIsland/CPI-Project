using ClubPenguin.Interactables;
using Disney.Kelowna.Common;
using System.Collections.Generic;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Interactables")]
	public class InteractionZoneCrowdObserver : FsmStateAction
	{
		[Tooltip("The crowd sizes to observe. Must be in ascending order. Do not use 0.")]
		public FsmInt[] CrowdSizeArray;

		[Tooltip("The start events. Must have the same number of elements as the stop events and crowd sizes.")]
		public FsmEvent[] StartEvents;

		[Tooltip("The stop events. Must have the same number of elements as the start events and crowd sizes.")]
		public FsmEvent[] StopEvents;

		private Stack<CrowdEvent> startEventStack = new Stack<CrowdEvent>();

		private Stack<CrowdEvent> stopEventStack = new Stack<CrowdEvent>();

		private EventChannel eventChannel;

		private int currentCount = 0;

		public override void Awake()
		{
			base.Awake();
			Initialize();
		}

		public override void OnEnter()
		{
			base.OnEnter();
			InteractiveZoneController componentInParent = base.Owner.GetComponentInParent<InteractiveZoneController>();
			if (componentInParent != null && componentInParent.Dispatcher != null)
			{
				eventChannel = new EventChannel(componentInParent.Dispatcher);
				componentInParent.Dispatcher.AddListener<InteractionZoneEvents.IteractiveItemCountEvent>(OnInteractiveItemCountChanged);
			}
		}

		public override void OnExit()
		{
			base.OnExit();
			if (eventChannel != null)
			{
				eventChannel.RemoveAllListeners();
			}
		}

		public override string ErrorCheck()
		{
			string text = base.ErrorCheck();
			if (CrowdSizeArray.Length != StartEvents.Length && StartEvents.Length != StopEvents.Length)
			{
				text += " You must configure the arrays to match in length.";
			}
			int num = 0;
			for (int i = 0; i < CrowdSizeArray.Length; i++)
			{
				if (CrowdSizeArray[i].Value <= num)
				{
					text += " The crowd sizes must be in ascending order of size.";
					break;
				}
				num = i;
			}
			return text;
		}

		private void Initialize()
		{
			currentCount = 0;
			stopEventStack.Clear();
			startEventStack.Clear();
			for (int num = StartEvents.Length - 1; num >= 0; num--)
			{
				startEventStack.Push(new CrowdEvent(StartEvents[num], StopEvents[num]));
			}
		}

		private bool OnInteractiveItemCountChanged(InteractionZoneEvents.IteractiveItemCountEvent evt)
		{
			int num = 0;
			foreach (int value in evt.InstrumentPlayCountDictionary.Values)
			{
				num += value;
			}
			FsmEvent fsmEvent = null;
			if (IsNewCountAtNewLevel(num))
			{
				if (num != 0)
				{
					for (int num2 = CrowdSizeArray.Length - 1; num2 >= 0; num2--)
					{
						if (num >= CrowdSizeArray[num2].Value)
						{
							fsmEvent = StartEvents[num2];
							break;
						}
					}
				}
				if (num < currentCount)
				{
					CrowdEvent crowdEvent = null;
					do
					{
						crowdEvent = stopEventStack.Peek();
						if (crowdEvent.startAnimationEvent == fsmEvent)
						{
							break;
						}
						base.Fsm.Event(crowdEvent.stopAnimationEvent);
						startEventStack.Push(crowdEvent);
						stopEventStack.Pop();
					}
					while (crowdEvent != null && stopEventStack.Count > 0);
				}
				else if (fsmEvent != null)
				{
					CrowdEvent crowdEvent2 = null;
					do
					{
						crowdEvent2 = startEventStack.Pop();
						base.Fsm.Event(crowdEvent2.startAnimationEvent);
						stopEventStack.Push(crowdEvent2);
					}
					while (crowdEvent2.startAnimationEvent != fsmEvent);
				}
			}
			currentCount = num;
			return false;
		}

		private bool IsNewCountAtNewLevel(int newCount)
		{
			if (newCount == currentCount)
			{
				return false;
			}
			if (newCount == 0 || currentCount == 0)
			{
				return true;
			}
			int indexOfCrowdLevel = GetIndexOfCrowdLevel(newCount);
			int indexOfCrowdLevel2 = GetIndexOfCrowdLevel(currentCount);
			return indexOfCrowdLevel != indexOfCrowdLevel2;
		}

		private int GetIndexOfCrowdLevel(int crowdSize)
		{
			int result = -1;
			for (int num = CrowdSizeArray.Length - 1; num >= 0; num--)
			{
				if (crowdSize >= CrowdSizeArray[num].Value)
				{
					result = num;
					break;
				}
			}
			return result;
		}

		public override string ToString()
		{
			return "Count = " + currentCount;
		}
	}
}
