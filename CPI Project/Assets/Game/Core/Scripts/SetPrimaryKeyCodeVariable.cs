using ClubPenguin.Input;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;

namespace ClubPenguin.Core
{
	public class SetPrimaryKeyCodeVariable : FsmStateAction
	{
		public SingleControlInputInfo.Actions ControlAction;

		[RequiredField]
		public FsmString Variable;

		public override void OnEnter()
		{
			SingleControlInputInfo singleControlInputInfo = new SingleControlInputInfo();
			singleControlInputInfo.ControlAction = ControlAction;
			SingleControlInputInfo singleControlInputInfo2 = singleControlInputInfo;
			Service.Get<InputService>().PopulateInputInfo(singleControlInputInfo2);
			Variable.Value = singleControlInputInfo2.PrimaryKey;
			Finish();
		}
	}
}
