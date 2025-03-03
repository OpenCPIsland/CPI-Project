using ClubPenguin.Core;
using ClubPenguin.Props;
using Disney.Kelowna.Common.DataModel;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Props")]
	public class GetPlayerHeldItem : FsmStateAction
	{
		public FsmString ObjectId;

		public override void OnEnter()
		{
			Prop component = base.Owner.GetComponent<Prop>();
			if (component != null && component.PropUserRef != null)
			{
				DataEntityHandle playerHandle = component.PropUserRef.PlayerHandle;
				if (!playerHandle.IsNull)
				{
					ObjectId.Value = ((Service.Get<CPDataEntityCollection>().GetComponent<HeldObjectsData>(playerHandle).HeldObject != null) ? Service.Get<CPDataEntityCollection>().GetComponent<HeldObjectsData>(playerHandle).HeldObject.ObjectId : "");
				}
			}
			else
			{
				ObjectId.Value = "";
			}
			Finish();
		}
	}
}
