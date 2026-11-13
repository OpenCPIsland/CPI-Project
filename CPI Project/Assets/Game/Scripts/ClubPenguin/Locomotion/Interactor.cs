using UnityEngine;

namespace ClubPenguin.Locomotion
{
	public class Interactor : MonoBehaviour
	{
		public struct InteractionInstance
		{
			public PlayMakerFSM fsm;

			public GameObject actionGraphOnExit;
		}

		private InteractionInstance instance;

		public void Set(FsmTemplate template, GameObject actionGraphOnExit = null)
		{
			instance.fsm = base.gameObject.AddComponent<PlayMakerFSM>();
			instance.fsm.SetFsmTemplate(template);
			instance.fsm.FsmName = template.name;
			instance.actionGraphOnExit = actionGraphOnExit;
		}

		public void Update()
		{
			if (instance.fsm != null && instance.fsm.Fsm.Finished)
			{
				if (instance.fsm != null)
				{
					Object.Destroy(instance.fsm);
				}
				if (instance.actionGraphOnExit != null)
				{
					SceneRefs.ActionSequencer.StartSequence(base.gameObject, instance.actionGraphOnExit);
				}
				Object.Destroy(this);
			}
		}
	}
}
