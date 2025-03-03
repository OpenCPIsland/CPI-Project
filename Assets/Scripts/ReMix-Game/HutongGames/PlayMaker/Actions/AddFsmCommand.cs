using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Interactables")]
	public class AddFsmCommand : FsmStateAction
	{
		public FsmOwnerDefault gameObject;

		public FsmTemplate Template;

		public string FsmName;

		public override void OnEnter()
		{
			GameObject gameObject = (this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : this.gameObject.GameObject.Value;
			if (gameObject != null)
			{
				if (HasFsmCommand.hasFsm(gameObject, Template, FsmName))
				{
					throw new InvalidOperationException("Attempting to add an Fsm that already exists.");
				}
				PlayMakerFSM playMakerFSM = gameObject.AddComponent<PlayMakerFSM>();
				playMakerFSM.SetFsmTemplate(Template);
				if (!string.IsNullOrEmpty(FsmName))
				{
					playMakerFSM.FsmName = FsmName;
				}
				else if (Template != null)
				{
					playMakerFSM.FsmName = Template.name;
				}
			}
			Finish();
		}
	}
}
