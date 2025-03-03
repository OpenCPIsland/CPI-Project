using ClubPenguin.Core;
using ClubPenguin.Igloo;
using Disney.Kelowna.Common.DataModel;
using Disney.Kelowna.Common.SEDFSM;
using Disney.MobileNetwork;

namespace ClubPenguin
{
	public class SetScreenStateHandler : AbstractMultiStateHandler
	{
		public SceneStateData.SceneState State;

		public bool ReturnToPreviousState;

		private CPDataEntityCollection dataEntityCollection;

		private SceneStateData.SceneState previousState;

		private void Awake()
		{
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
		}

		protected override void OnEnter()
		{
			DataEntityHandle handle = dataEntityCollection.FindEntityByName("ActiveSceneData");
			SceneStateData component;
			if (dataEntityCollection.TryGetComponent(handle, out component))
			{
				previousState = component.State;
				component.State = State;
			}
		}

		protected override void OnExit()
		{
			if (ReturnToPreviousState)
			{
				DataEntityHandle handle = dataEntityCollection.FindEntityByName("ActiveSceneData");
				SceneStateData component;
				if (dataEntityCollection.TryGetComponent(handle, out component))
				{
					component.State = previousState;
				}
			}
		}
	}
}
