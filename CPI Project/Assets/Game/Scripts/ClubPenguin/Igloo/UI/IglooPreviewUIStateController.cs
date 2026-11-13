using ClubPenguin.Core;

namespace ClubPenguin.Igloo.UI
{
	public class IglooPreviewUIStateController : AbstractIglooUIState
	{
		private IglooUIStateController stateController;

		public void Init(IglooUIStateController stateController)
		{
			this.stateController = stateController;
		}

		public override void OnEnter()
		{
			eventDispatcher.DispatchEvent(new PlayerSpawnedEvents.LocalPlayerReadyToSpawn(dataEntityCollection.LocalPlayerHandle));
			eventChannel.AddListener<IglooUIEvents.SetStateButtonPressed>(onSceneStateButton);
		}

		public override void OnExit()
		{
			eventChannel.RemoveAllListeners();
		}

		private bool onSceneStateButton(IglooUIEvents.SetStateButtonPressed evt)
		{
			SceneLayoutData activeSceneLayoutData = stateController.DataManager.LayoutManager.GetActiveSceneLayoutData();
			switch (evt.SceneState)
			{
			case SceneStateData.SceneState.Play:
				if (activeSceneLayoutData.LayoutId == stateController.ActiveIglooId)
				{
					stateController.TransitionFromEditOrPreviewToPlay(true);
				}
				else
				{
					stateController.ShowConfirmPublishPrompt();
				}
				break;
			case SceneStateData.SceneState.Edit:
				stateController.TransitionFromPreviewToEdit();
				break;
			}
			return false;
		}
	}
}
