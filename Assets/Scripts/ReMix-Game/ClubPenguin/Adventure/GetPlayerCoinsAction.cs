using ClubPenguin.Core;
using Disney.Kelowna.Common.DataModel;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Quest")]
	public class GetPlayerCoinsAction : FsmStateAction
	{
		public FsmInt OUT_CoinCount;

		public override void OnEnter()
		{
			DataEntityHandle localPlayerHandle = Service.Get<CPDataEntityCollection>().LocalPlayerHandle;
			OUT_CoinCount.Value = Service.Get<CPDataEntityCollection>().GetComponent<CoinsData>(localPlayerHandle).Coins;
			Finish();
		}
	}
}
