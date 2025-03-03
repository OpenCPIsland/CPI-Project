using HutongGames.PlayMaker;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Locomotion")]
	public class MovePlayerToMascotAction : FsmStateAction
	{
		public string MascotName;

		public override void OnEnter()
		{
			GameObject gameObject = GameObject.Find(MascotName);
			if (gameObject != null)
			{
				Transform transform = gameObject.transform.Find("DialogSpot");
				base.Owner.transform.position = transform.position;
				base.Owner.transform.rotation = transform.rotation;
			}
			Finish();
		}
	}
}
