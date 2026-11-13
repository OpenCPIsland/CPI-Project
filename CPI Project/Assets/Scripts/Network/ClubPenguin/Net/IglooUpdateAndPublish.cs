using ClubPenguin.Net.Domain.Igloo;
using ClubPenguin.Net.Domain.Scene;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;

namespace ClubPenguin.Net
{
	public class IglooUpdateAndPublish : IIglooUpdateLayoutErrorHandler, IIglooUpdateDataErrorHandler
	{
		private const int SUCCESS_COUNT_TOTAL = 2;

		private IIglooService iglooService;

		private IIglooUpdateAndPublishErrorHandler errorHandler;

		private EventChannel eventChannel;

		private long layoutId;

		private IglooVisibility? visibility;

		private MutableSceneLayout sceneLayout;

		private int failCount = 0;

		private int successCount = 0;

		private SavedSceneLayout savedSceneLayout;

		public IglooUpdateAndPublish(IIglooService iglooService, IIglooUpdateAndPublishErrorHandler errorHandler, long layoutId, IglooVisibility? visibility, MutableSceneLayout sceneLayout)
		{
			this.iglooService = iglooService;
			this.errorHandler = errorHandler;
			this.layoutId = layoutId;
			this.visibility = visibility;
			this.sceneLayout = sceneLayout;
			eventChannel = new EventChannel(Service.Get<EventDispatcher>());
		}

		public void Execute()
		{
			eventChannel.AddListener<IglooServiceEvents.IglooLayoutUpdated>(onLayoutUpdated);
			eventChannel.AddListener<IglooServiceEvents.IglooDataUpdated>(onDataUpdated);
			iglooService.UpdateIglooLayout(layoutId, sceneLayout, this);
			iglooService.UpdateIglooData(visibility, layoutId, this);
		}

		private bool onLayoutUpdated(IglooServiceEvents.IglooLayoutUpdated evt)
		{
			eventChannel.RemoveListener<IglooServiceEvents.IglooLayoutUpdated>(onLayoutUpdated);
			savedSceneLayout = evt.SavedSceneLayout;
			successCount++;
			checkStatus();
			return false;
		}

		private bool onDataUpdated(IglooServiceEvents.IglooDataUpdated evt)
		{
			eventChannel.RemoveListener<IglooServiceEvents.IglooDataUpdated>(onDataUpdated);
			successCount++;
			checkStatus();
			return false;
		}

		public void OnUpdateLayoutError()
		{
			failCount++;
			checkStatus();
		}

		public void OnUpdateDataError()
		{
			failCount++;
			checkStatus();
		}

		private void checkStatus()
		{
			if (failCount > 0)
			{
				eventChannel.RemoveAllListeners();
				errorHandler.OnUpdateAndPublishError();
			}
			else if (successCount == 2)
			{
				Service.Get<EventDispatcher>().DispatchEvent(new IglooServiceEvents.IglooPublished(savedSceneLayout));
			}
		}
	}
}
