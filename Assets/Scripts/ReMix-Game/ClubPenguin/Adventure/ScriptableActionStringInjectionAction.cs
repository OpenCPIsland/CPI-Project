using HutongGames.PlayMaker;

namespace ClubPenguin.Adventure
{
	public class ScriptableActionStringInjectionAction : ScriptableActionInjectionAction<FsmString>
	{
		protected override object getDataObject()
		{
			return DataObject.Value;
		}
	}
}
