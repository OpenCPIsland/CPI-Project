using HutongGames.PlayMaker;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	[RequireComponent(typeof(Collider))]
	public class QuestTriggerRemoteFSM : MonoBehaviour
	{
		public List<Fsm> RemoteFsmList;

		public string EnterEvent;

		public string StayEvent;

		public string ExitEvent;

		public string Tag;

		private void SendEvent(string evt)
		{
			if (RemoteFsmList != null)
			{
				for (int i = 0; i < RemoteFsmList.Count; i++)
				{
					RemoteFsmList[i].Event(evt);
				}
			}
		}

		public void OnTriggerEnter(Collider col)
		{
			if (!string.IsNullOrEmpty(EnterEvent) && col.CompareTag(Tag))
			{
				SendEvent(EnterEvent);
			}
		}

		public void OnTriggerStay(Collider col)
		{
			if (!string.IsNullOrEmpty(StayEvent) && col.CompareTag(Tag))
			{
				SendEvent(StayEvent);
			}
		}

		public void OnTriggerExit(Collider col)
		{
			if (!string.IsNullOrEmpty(ExitEvent) && col.CompareTag(Tag))
			{
				SendEvent(ExitEvent);
			}
		}
	}
}
