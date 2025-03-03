using ClubPenguin.Net.Client;
using ClubPenguin.Net.Domain;
using ClubPenguin.Net.Domain.Decoration;
using ClubPenguin.Net.Domain.Igloo;
using ClubPenguin.Net.Domain.Scene;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using hg.ApiWebKit.core.http;
using System.Collections.Generic;

namespace ClubPenguin.Net
{
	public class IglooService : BaseNetworkService, IIglooService, INetworkService
	{
		protected override void setupListeners()
		{
			clubPenguinClient.GameServer.AddEventListener(GameServerEvent.IGLOO_UPDATED, onIglooUpdated);
			clubPenguinClient.GameServer.AddEventListener(GameServerEvent.FORCE_LEAVE, onForceLeave);
		}

		public void CreateIglooLayout(MutableSceneLayout sceneLayout, IIglooCreateErrorHandler errorHandler = null)
		{
			APICall<CreateIglooLayoutOperation> aPICall = clubPenguinClient.IglooApi.CreateIglooLayout(sceneLayout);
			aPICall.OnResponse += onCreateIglooLayout;
			aPICall.OnError += delegate(CreateIglooLayoutOperation op, HttpResponse HttpResponse)
			{
				if (errorHandler != null)
				{
					errorHandler.OnCreateLayoutError();
				}
				else
				{
					handleCPResponseError(op, HttpResponse);
				}
			};
			aPICall.Execute();
		}

		private void onCreateIglooLayout(CreateIglooLayoutOperation operation, HttpResponse httpResponse)
		{
			SavedSceneLayout responseBody = operation.ResponseBody;
			Service.Get<EventDispatcher>().DispatchEvent(new IglooServiceEvents.IglooLayoutCreated(responseBody));
		}

		public void UpdateAndPublish(long layoutId, IglooVisibility? visibility, MutableSceneLayout sceneLayout, IIglooUpdateAndPublishErrorHandler errorHandler)
		{
			IglooUpdateAndPublish iglooUpdateAndPublish = new IglooUpdateAndPublish(this, errorHandler, layoutId, visibility, sceneLayout);
			iglooUpdateAndPublish.Execute();
		}

		public void UpdateIglooLayout(long layoutId, MutableSceneLayout sceneLayout, IIglooUpdateLayoutErrorHandler errorHandler = null)
		{
			APICall<UpdateIglooLayoutOperation> aPICall = clubPenguinClient.IglooApi.UpdateIglooLayout(layoutId, sceneLayout);
			aPICall.OnResponse += onUpdateIglooLayout;
			aPICall.OnError += delegate(UpdateIglooLayoutOperation op, HttpResponse HttpResponse)
			{
				if (errorHandler != null)
				{
					errorHandler.OnUpdateLayoutError();
				}
				else
				{
					handleCPResponseError(op, HttpResponse);
				}
			};
			aPICall.Execute();
		}

		private void onUpdateIglooLayout(UpdateIglooLayoutOperation operation, HttpResponse httpResponse)
		{
			SavedSceneLayout responseBody = operation.ResponseBody;
			Service.Get<EventDispatcher>().DispatchEvent(new IglooServiceEvents.IglooLayoutUpdated(responseBody));
		}

		public void DeleteIglooLayout(long layoutId, IIglooDeleteLayoutErrorHandler errorHandler = null)
		{
			APICall<DeleteIglooLayoutOperation> aPICall = clubPenguinClient.IglooApi.DeleteIglooLayout(layoutId);
			aPICall.OnResponse += onDeleteIglooLayout;
			aPICall.OnError += delegate(DeleteIglooLayoutOperation op, HttpResponse HttpResponse)
			{
				if (errorHandler != null)
				{
					errorHandler.OnDeleteLayoutError();
				}
				else
				{
					handleCPResponseError(op, HttpResponse);
				}
			};
			aPICall.Execute();
		}

		private void onDeleteIglooLayout(DeleteIglooLayoutOperation operation, HttpResponse httpResponse)
		{
			long sceneLayoutId = operation.SceneLayoutId;
			Service.Get<EventDispatcher>().DispatchEvent(new IglooServiceEvents.IglooLayoutDeleted(sceneLayoutId));
		}

		public void GetIglooLayout(long layoutId)
		{
			APICall<GetIglooLayoutOperation> iglooLayout = clubPenguinClient.IglooApi.GetIglooLayout(layoutId);
			iglooLayout.OnResponse += onGetIglooLayout;
			iglooLayout.OnError += handleCPResponseError;
			iglooLayout.Execute();
		}

		private void onGetIglooLayout(GetIglooLayoutOperation operation, HttpResponse httpResponse)
		{
			SavedSceneLayout responseBody = operation.ResponseBody;
			Service.Get<EventDispatcher>().DispatchEvent(new IglooServiceEvents.IglooLayoutLoaded(responseBody));
		}

		public void GetFriendsIgloos(string language)
		{
			APICall<GetFriendsIgloosOperation> friendsIgloos = clubPenguinClient.IglooApi.GetFriendsIgloos(language);
			friendsIgloos.OnResponse += onGetFriendsIgloos;
			friendsIgloos.OnError += handleCPResponseError;
			friendsIgloos.Execute();
		}

		private void onGetFriendsIgloos(GetFriendsIgloosOperation operation, HttpResponse httpResponse)
		{
			List<IglooListItem> iglooListItems = operation.IglooListItems;
			Service.Get<EventDispatcher>().DispatchEvent(new IglooServiceEvents.FriendIgloosListLoaded(iglooListItems));
		}

		public void GetPopularIgloos(string language)
		{
			APICall<GetPopularIgloosOperation> popularIgloos = clubPenguinClient.IglooApi.GetPopularIgloos(language);
			popularIgloos.OnResponse += onGetPopularIgloos;
			popularIgloos.OnError += handleCPResponseError;
			popularIgloos.Execute();
		}

		private void onGetPopularIgloos(GetPopularIgloosOperation operation, HttpResponse httpResponse)
		{
			List<IglooListItem> iglooListItems = operation.IglooListItems;
			Service.Get<EventDispatcher>().DispatchEvent(new IglooServiceEvents.PopularIgloosListLoaded(iglooListItems));
		}

		public void GetIglooPopulationsByZoneIds(string language, IList<ZoneId> zoneIds, List<DataEntityHandle> handles)
		{
			APICall<GetIglooPopulationsByZoneIdsOperation> iglooPopulationsByZoneIds = clubPenguinClient.IglooApi.GetIglooPopulationsByZoneIds(language, zoneIds, handles);
			iglooPopulationsByZoneIds.OnResponse += onGetIglooPopulationsByZoneIds;
			iglooPopulationsByZoneIds.OnError += handleCPResponseError;
			iglooPopulationsByZoneIds.Execute();
		}

		private void onGetIglooPopulationsByZoneIds(GetIglooPopulationsByZoneIdsOperation operation, HttpResponse httpResponse)
		{
			IglooServiceEvents.IgloosFromZoneIdsLoaded evt = new IglooServiceEvents.IgloosFromZoneIdsLoaded(operation.RoomPopulations, operation.Handles);
			Service.Get<EventDispatcher>().DispatchEvent(evt);
		}

		public void GetActiveIglooLayout(string iglooId)
		{
			APICall<GetActiveIglooLayoutOperation> activeIglooLayout = clubPenguinClient.IglooApi.GetActiveIglooLayout(iglooId);
			activeIglooLayout.OnResponse += onGetActiveIglooLayout;
			activeIglooLayout.OnError += handleCPResponseError;
			activeIglooLayout.Execute();
		}

		private void onGetActiveIglooLayout(GetActiveIglooLayoutOperation operation, HttpResponse httpResponse)
		{
			SceneLayout responseBody = operation.ResponseBody;
			Service.Get<EventDispatcher>().DispatchEvent(new IglooServiceEvents.IglooActiveLayoutLoaded(responseBody));
		}

		public void UpdateIglooData(IglooVisibility? visibility, long? activeLayoutId, IIglooUpdateDataErrorHandler errorHandler = null)
		{
			MutableIglooData mutableIglooData = new MutableIglooData();
			mutableIglooData.activeLayoutId = activeLayoutId;
			mutableIglooData.visibility = visibility;
			APICall<UpdateIglooDataOperation> aPICall = clubPenguinClient.IglooApi.UpdateIglooData(mutableIglooData);
			aPICall.OnResponse += onIglooDataUpdated;
			aPICall.OnError += delegate(UpdateIglooDataOperation op, HttpResponse HttpResponse)
			{
				if (errorHandler != null)
				{
					errorHandler.OnUpdateDataError();
				}
				else
				{
					handleCPResponseError(op, HttpResponse);
				}
			};
			aPICall.Execute();
		}

		private void onIglooDataUpdated(UpdateIglooDataOperation operation, HttpResponse httpResponse)
		{
			SignedResponse<IglooData> signedResponseBody = operation.SignedResponseBody;
			clubPenguinClient.GameServer.SendIglooUpdated(signedResponseBody);
			Service.Get<EventDispatcher>().DispatchEvent(new IglooServiceEvents.IglooDataUpdated(signedResponseBody));
		}

		private void onIglooUpdated(GameServerEvent gameServerEvent, object data)
		{
			string iglooId = (string)data;
			Service.Get<EventDispatcher>().DispatchEvent(new IglooServiceEvents.IglooUpdated(iglooId));
		}

		private void onForceLeave(GameServerEvent gameServerEvent, object data)
		{
			ZoneId zoneId = (ZoneId)data;
			Service.Get<EventDispatcher>().DispatchEvent(new IglooServiceEvents.ForceLeave(zoneId));
		}

		public void CreateDecoration(int definitionId, DecorationType type, int? count)
		{
			APICall<CreateDecorationOperation> aPICall = clubPenguinClient.IglooApi.CreateDecoration(definitionId, type, count);
			aPICall.OnResponse += onDecorationCreated;
			aPICall.OnError += handleCPResponseError;
			aPICall.Execute();
		}

		private void onDecorationCreated(CreateDecorationOperation operation, HttpResponse httpResponse)
		{
			CreateDecorationResponse responseBody = operation.ResponseBody;
			Service.Get<EventDispatcher>().DispatchEvent(new IglooServiceEvents.DecorationCreated(responseBody.decorationId));
			Service.Get<EventDispatcher>().DispatchEvent(new RewardServiceEvents.MyAssetsReceived(responseBody.assets));
			handleCPResponse(responseBody);
		}

		public void PurchaseDecoration(int decorationDefinitionId, DecorationType type, int? count, IIglooCatalogPurchaseErrorHandler errorHandler)
		{
			PurchaseDecoration(new DecorationId(decorationDefinitionId, type), count, errorHandler);
		}

		public void PurchaseDecoration(DecorationId decorationId, int? count, IIglooCatalogPurchaseErrorHandler errorHandler)
		{
			APICall<PurchaseDecorationOperation> aPICall = clubPenguinClient.IglooApi.PurchaseDecoration(decorationId, count);
			aPICall.OnResponse += onDecorationUpdated;
			aPICall.OnError += delegate
			{
				errorHandler.OnPurchaseDecorationError();
			};
			aPICall.Execute();
		}

		private void onDecorationUpdated(PurchaseDecorationOperation operation, HttpResponse httpResponse)
		{
			UpdateDecorationResponse responseBody = operation.ResponseBody;
			Service.Get<EventDispatcher>().DispatchEvent(new IglooServiceEvents.DecorationUpdated(responseBody.decorationId, operation.Count));
			Service.Get<EventDispatcher>().DispatchEvent(new RewardServiceEvents.MyAssetsReceived(responseBody.assets));
			handleCPResponse(responseBody);
		}

		public void GetDecorations()
		{
			APICall<GetDecorationsOperation> decorations = clubPenguinClient.IglooApi.GetDecorations();
			decorations.OnResponse += onDecorationsLoaded;
			decorations.OnError += handleCPResponseError;
			decorations.Execute();
		}

		private void onDecorationsLoaded(GetDecorationsOperation operation, HttpResponse httpResponse)
		{
			DecorationInventory decorationInventory = operation.DecorationInventory;
			Service.Get<EventDispatcher>().DispatchEvent(new IglooServiceEvents.DecorationsLoaded(decorationInventory));
		}

		public void DeleteDecoration(int decorationDefinitionId, DecorationType type)
		{
			DeleteDecoration(new DecorationId(decorationDefinitionId, type));
		}

		public void DeleteDecoration(DecorationId decorationId)
		{
			APICall<DeleteDecorationOperation> aPICall = clubPenguinClient.IglooApi.DeleteDecoration(decorationId);
			aPICall.OnResponse += onDecorationDeleted;
			aPICall.OnError += handleCPResponseError;
			aPICall.Execute();
		}

		private void onDecorationDeleted(DeleteDecorationOperation operation, HttpResponse httpResponse)
		{
			DecorationId decorationId = DecorationId.FromString(operation.DecorationId);
			Service.Get<EventDispatcher>().DispatchEvent(new IglooServiceEvents.DecorationDeleted(decorationId));
		}

		public void QA_AddDecorations(int decorationDefinitionId, DecorationType type, int count)
		{
			APICall<QACreateDecorationOperation> aPICall = clubPenguinClient.IglooApi.QaCreateDecoration(decorationDefinitionId, type, count);
			aPICall.OnResponse += onQaDecorationCreated;
			aPICall.OnError += handleCPResponseError;
			aPICall.Execute();
		}

		private void onQaDecorationCreated(QACreateDecorationOperation operation, HttpResponse httpResponse)
		{
			CreateDecorationResponse responseBody = operation.ResponseBody;
			Service.Get<EventDispatcher>().DispatchEvent(new IglooServiceEvents.DecorationCreated(responseBody.decorationId));
			Service.Get<EventDispatcher>().DispatchEvent(new RewardServiceEvents.MyAssetsReceived(responseBody.assets));
			handleCPResponse(responseBody);
		}

		public void QA_DeleteAllDecorations()
		{
			APICall<QADeleteAllDecorationsOperation> aPICall = clubPenguinClient.IglooApi.QaDeleteAllDecorations();
			aPICall.OnError += handleCPResponseError;
			aPICall.Execute();
		}

		public void QA_DeleteAllIgloos()
		{
			APICall<QADeleteAllIgloosOperation> aPICall = clubPenguinClient.IglooApi.QaDeleteAllIgloos();
			aPICall.Execute();
		}
	}
}
