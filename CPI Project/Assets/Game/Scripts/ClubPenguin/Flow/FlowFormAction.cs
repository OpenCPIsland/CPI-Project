using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.Flow
{
	public class FlowFormAction : ScriptableAction
	{
		public GameObject FormPrefab;

		public override IEnumerator Execute(ScriptableActionPlayer player)
		{
			GameObject form = Object.Instantiate(FormPrefab);
			form.name = base.name + "Form";
			FlowFormController controller = form.GetComponent<FlowFormController>();
			Service.Get<EventDispatcher>().DispatchEvent(new PopupEvents.ShowPopup(form));
			while (!controller.IsFinished)
			{
				yield return null;
			}
			player.NextAction = controller.NextAction;
			Object.Destroy(form);
		}
	}
}
