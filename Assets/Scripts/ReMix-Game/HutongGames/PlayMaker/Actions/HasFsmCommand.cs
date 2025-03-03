using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Interactables")]
	public class HasFsmCommand : FsmStateAction
	{
		public FsmOwnerDefault gameObject;

		public FsmTemplate Template;

		public string FsmName;

		public FsmEvent HasFsmEvent;

		public FsmEvent DoesNotHaveFsmEvent;

		public static bool hasFsm(GameObject go, FsmTemplate Template, string FsmName)
		{
			if (go != null)
			{
				PlayMakerFSM[] components = go.GetComponents<PlayMakerFSM>();
				for (int i = 0; i < components.Length; i++)
				{
					if (!string.IsNullOrEmpty(FsmName))
					{
						if (components[i].FsmName == FsmName)
						{
							return true;
						}
					}
					else if (components[i].FsmTemplate == Template)
					{
						return true;
					}
				}
			}
			return false;
		}

		public override void OnEnter()
		{
			GameObject go = (gameObject.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : gameObject.GameObject.Value;
			if (hasFsm(go, Template, FsmName))
			{
				base.Fsm.Event(HasFsmEvent);
			}
			else
			{
				base.Fsm.Event(DoesNotHaveFsmEvent);
			}
			Finish();
		}
	}
}
