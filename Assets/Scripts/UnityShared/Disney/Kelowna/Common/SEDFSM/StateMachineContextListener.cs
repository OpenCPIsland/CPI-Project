using System;
using System.Collections;
using UnityEngine;

namespace Disney.Kelowna.Common.SEDFSM
{
	[DisallowMultipleComponent]
	public class StateMachineContextListener : MonoBehaviour
	{
		public GameObject ContextTarget;

		[HideInInspector]
		public StateMachineContext Context;

		public event Action<StateMachineContext> OnContextAdded;

		private IEnumerator Start()
		{
			while (Context == null)
			{
				yield return null;
				Context = ContextTarget.GetComponentInChildren<StateMachineContext>();
				if (Context != null && this.OnContextAdded != null)
				{
					yield return new WaitForEndOfFrame();
					this.OnContextAdded(Context);
				}
			}
		}
	}
}
