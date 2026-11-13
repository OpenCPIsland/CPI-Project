using ClubPenguin.Net.Domain;
using ClubPenguin.Net.Domain.Decoration;
using ClubPenguin.Net.Domain.Igloo;
using ClubPenguin.Net.Domain.Scene;
using Disney.Kelowna.Common.DataModel;
using System.Collections.Generic;

namespace ClubPenguin.Net.Client
{
	public class IglooApi
	{
		private ClubPenguinClient clubPenguinClient;

		public IglooApi(ClubPenguinClient clubPenguinClient)
		{
			this.clubPenguinClient = clubPenguinClient;
		}

		public APICall<CreateIglooLayoutOperation> CreateIglooLayout(MutableSceneLayout sceneLayout)
		{
			CreateIglooLayoutOperation operation = new CreateIglooLayoutOperation(sceneLayout);
			return new APICall<CreateIglooLayoutOperation>(clubPenguinClient, operation);
		}

		public APICall<UpdateIglooLayoutOperation> UpdateIglooLayout(long sceneLayoutId, MutableSceneLayout sceneLayout)
		{
			UpdateIglooLayoutOperation operation = new UpdateIglooLayoutOperation(sceneLayoutId, sceneLayout);
			return new APICall<UpdateIglooLayoutOperation>(clubPenguinClient, operation);
		}

		public APICall<DeleteIglooLayoutOperation> DeleteIglooLayout(long sceneLayoutId)
		{
			DeleteIglooLayoutOperation operation = new DeleteIglooLayoutOperation(sceneLayoutId);
			return new APICall<DeleteIglooLayoutOperation>(clubPenguinClient, operation);
		}

		public APICall<GetIglooLayoutOperation> GetIglooLayout(long sceneLayoutId)
		{
			GetIglooLayoutOperation operation = new GetIglooLayoutOperation(sceneLayoutId);
			return new APICall<GetIglooLayoutOperation>(clubPenguinClient, operation);
		}

		public APICall<GetFriendsIgloosOperation> GetFriendsIgloos(string language)
		{
			GetFriendsIgloosOperation operation = new GetFriendsIgloosOperation(language);
			return new APICall<GetFriendsIgloosOperation>(clubPenguinClient, operation);
		}

		public APICall<GetPopularIgloosOperation> GetPopularIgloos(string language)
		{
			GetPopularIgloosOperation operation = new GetPopularIgloosOperation(language);
			return new APICall<GetPopularIgloosOperation>(clubPenguinClient, operation);
		}

		public APICall<GetIglooPopulationsByZoneIdsOperation> GetIglooPopulationsByZoneIds(string language, IList<ZoneId> zoneIds, List<DataEntityHandle> handles)
		{
			GetIglooPopulationsByZoneIdsOperation operation = new GetIglooPopulationsByZoneIdsOperation(language, zoneIds, handles);
			return new APICall<GetIglooPopulationsByZoneIdsOperation>(clubPenguinClient, operation);
		}

		public APICall<GetActiveIglooLayoutOperation> GetActiveIglooLayout(string iglooId)
		{
			GetActiveIglooLayoutOperation operation = new GetActiveIglooLayoutOperation(iglooId);
			return new APICall<GetActiveIglooLayoutOperation>(clubPenguinClient, operation);
		}

		public APICall<UpdateIglooDataOperation> UpdateIglooData(MutableIglooData update)
		{
			UpdateIglooDataOperation operation = new UpdateIglooDataOperation(update);
			return new APICall<UpdateIglooDataOperation>(clubPenguinClient, operation);
		}

		public APICall<UpdateIglooDataOperation> SetActiveIglooLayout(long iglooLayoutId)
		{
			MutableIglooData mutableIglooData = new MutableIglooData();
			mutableIglooData.activeLayoutId = iglooLayoutId;
			UpdateIglooDataOperation operation = new UpdateIglooDataOperation(mutableIglooData);
			return new APICall<UpdateIglooDataOperation>(clubPenguinClient, operation);
		}

		public APICall<UpdateIglooDataOperation> SetIglooVisibility(IglooVisibility visibility)
		{
			MutableIglooData mutableIglooData = new MutableIglooData();
			mutableIglooData.visibility = visibility;
			UpdateIglooDataOperation operation = new UpdateIglooDataOperation(mutableIglooData);
			return new APICall<UpdateIglooDataOperation>(clubPenguinClient, operation);
		}

		public APICall<CreateDecorationOperation> CreateDecoration(int definitionId, DecorationType type, int? count)
		{
			if (!count.HasValue)
			{
				count = 1;
			}
			CreateDecorationOperation operation = new CreateDecorationOperation(definitionId, type, count.Value);
			return new APICall<CreateDecorationOperation>(clubPenguinClient, operation);
		}

		public APICall<PurchaseDecorationOperation> PurchaseDecoration(DecorationId decorationId, int? count)
		{
			if (!count.HasValue)
			{
				count = 1;
			}
			PurchaseDecorationOperation operation = new PurchaseDecorationOperation(decorationId.ToString(), count.Value);
			return new APICall<PurchaseDecorationOperation>(clubPenguinClient, operation);
		}

		public APICall<GetDecorationsOperation> GetDecorations()
		{
			GetDecorationsOperation operation = new GetDecorationsOperation();
			return new APICall<GetDecorationsOperation>(clubPenguinClient, operation);
		}

		public APICall<DeleteDecorationOperation> DeleteDecoration(DecorationId decorationId)
		{
			DeleteDecorationOperation operation = new DeleteDecorationOperation(decorationId.ToString());
			return new APICall<DeleteDecorationOperation>(clubPenguinClient, operation);
		}

		public APICall<QACreateDecorationOperation> QaCreateDecoration(int definitionId, DecorationType type, int? count)
		{
			if (!count.HasValue)
			{
				count = 1;
			}
			QACreateDecorationOperation operation = new QACreateDecorationOperation(definitionId, type, count.Value);
			return new APICall<QACreateDecorationOperation>(clubPenguinClient, operation);
		}

		public APICall<QADeleteAllDecorationsOperation> QaDeleteAllDecorations()
		{
			QADeleteAllDecorationsOperation operation = new QADeleteAllDecorationsOperation();
			return new APICall<QADeleteAllDecorationsOperation>(clubPenguinClient, operation);
		}

		public APICall<QADeleteAllIgloosOperation> QaDeleteAllIgloos()
		{
			QADeleteAllIgloosOperation operation = new QADeleteAllIgloosOperation();
			return new APICall<QADeleteAllIgloosOperation>(clubPenguinClient, operation);
		}
	}
}
