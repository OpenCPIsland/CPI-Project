using HutongGames.PlayMaker;

namespace ClubPenguin.Adventure
{
	public class ScriptableActionIntInjectionAction : ScriptableActionInjectionAction<FsmInt>
	{
		protected override object getDataObject()
		{
			return DataObject.Value;
		}
	}
}
