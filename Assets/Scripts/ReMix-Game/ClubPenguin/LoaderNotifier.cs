using Disney.Kelowna.Common;
using Disney.Kelowna.Common.SEDFSM;
using System.Collections;
using UnityEngine;

namespace ClubPenguin
{
	public class LoaderNotifier : MonoBehaviour
	{
		[SerializeField]
		private string fsmTarget;

		private StateMachineContext smContext;

		private void OnEnable()
		{
			CoroutineRunner.Start(sendLoadedEvent(), this, "sendLoadedEvent");
		}

		private IEnumerator sendLoadedEvent()
		{
			while (smContext == null)
			{
				yield return null;
				smContext = GetComponentInParent<StateMachineContext>();
			}
			smContext.SendEvent(new ExternalEvent(fsmTarget, "loaded"));
		}

		private void OnDisable()
		{
			if (smContext != null)
			{
				smContext.SendEvent(new ExternalEvent(fsmTarget, "unloaded"));
			}
		}
	}
}
