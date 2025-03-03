using ClubPenguin.Core;
using Disney.Kelowna.Common.SEDFSM;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class DivingScreenWipe : MonoBehaviour
	{
		public void OnAnimationComplete()
		{
			GameObject gameObject = GameObject.FindWithTag(UIConstants.Tags.UI_Tray_Root);
			if (gameObject != null)
			{
				StateMachineContext component = gameObject.GetComponent<StateMachineContext>();
				if (component != null)
				{
					component.SendEvent(new ExternalEvent("Root", "restoreUI"));
				}
			}
			Object.Destroy(base.gameObject);
		}
	}
}
