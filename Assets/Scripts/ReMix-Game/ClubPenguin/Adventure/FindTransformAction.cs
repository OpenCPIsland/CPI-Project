using HutongGames.PlayMaker;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Quest")]
	[HutongGames.PlayMaker.Tooltip("NOTE: This action does not recursively search the parents hierarchy. Use 'name/gameobject' to search lower levels")]
	public class FindTransformAction : FsmStateAction
	{
		[RequiredField]
		public string ParentName;

		[RequiredField]
		public string ObjectName;

		public FsmGameObject OUT_GameObject;

		public override void OnEnter()
		{
			GameObject gameObject = GameObject.Find(ParentName);
			if (gameObject != null)
			{
				OUT_GameObject.Value = gameObject.transform.Find(ObjectName).gameObject;
			}
			Finish();
		}
	}
}
