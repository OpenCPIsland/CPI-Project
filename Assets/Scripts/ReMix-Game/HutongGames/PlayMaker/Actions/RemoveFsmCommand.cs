using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Interactables")]
	public class RemoveFsmCommand : FsmStateAction
	{
		public FsmOwnerDefault gameObject;

		public FsmTemplate Template;

		public string FsmName;

		public override void OnEnter()
		{
			if (Template != null)
			{
				GameObject gameObject = (this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : this.gameObject.GameObject.Value;
				if (gameObject != null)
				{
					PlayMakerFSM[] components = gameObject.GetComponents<PlayMakerFSM>();
					PlayMakerFSM playMakerFSM = null;
					for (int i = 0; i < components.Length; i++)
					{
						if (!string.IsNullOrEmpty(FsmName))
						{
							if (components[i].FsmName == FsmName)
							{
								playMakerFSM = components[i];
								break;
							}
						}
						else if (components[i].FsmTemplate == Template)
						{
							playMakerFSM = components[i];
							break;
						}
					}
					if (playMakerFSM != null)
					{
						UnityEngine.Object.Destroy(playMakerFSM);
					}
				}
				Finish();
				return;
			}
			throw new InvalidOperationException("Removing an Fsm with a null Template.");
		}
	}
}
