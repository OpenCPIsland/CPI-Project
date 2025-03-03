using HutongGames.PlayMaker;

namespace ClubPenguin.Adventure
{
	public class ScriptableActionGameObjectInjectionAction : ScriptableActionInjectionAction<FsmGameObject>
	{
		protected override object getDataObject()
		{
			return DataObject.Value;
		}
	}
}
