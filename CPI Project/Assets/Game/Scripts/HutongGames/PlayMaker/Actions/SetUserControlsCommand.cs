using ClubPenguin.Locomotion;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Interactables")]
	public class SetUserControlsCommand : FsmStateAction
	{
		public FsmOwnerDefault gameObject;

		public bool EnableControls;

		public override void OnEnter()
		{
			GameObject gameObject = (this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : this.gameObject.GameObject.Value;
			if (gameObject != null)
			{
				PenguinUserControl component = gameObject.GetComponent<PenguinUserControl>();
				if (component != null)
				{
					component.enabled = EnableControls;
				}
			}
			Finish();
		}
	}
}
