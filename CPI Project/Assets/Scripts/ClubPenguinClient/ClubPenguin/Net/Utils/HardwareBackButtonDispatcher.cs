using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ClubPenguin.Net.Utils
{
	[RequireComponent(typeof(EventSystem))]
	public class HardwareBackButtonDispatcher : MonoBehaviour
	{
		private static Stack<Button> pointerHandlerStack = new Stack<Button>();

		private static bool attachedToEventSystem = false;

		public static bool ListenForInput = true;

		public static void SetTargetClickHandler(Button pointerClickHandler, bool visible = true)
		{
			if (!attachedToEventSystem)
			{
				throw new Exception("HardwareBackButtonDispatcher script has not been attached to an EventSystem.");
			}
			if (!(pointerClickHandler.gameObject == null))
			{
				pointerHandlerStack.Push(pointerClickHandler);
			}
		}

		private void Awake()
		{
			attachedToEventSystem = true;
		}
	}
}
