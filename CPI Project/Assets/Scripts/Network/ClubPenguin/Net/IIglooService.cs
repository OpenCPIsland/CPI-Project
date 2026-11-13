using ClubPenguin.Net.Domain;
using ClubPenguin.Net.Domain.Decoration;
using ClubPenguin.Net.Domain.Igloo;
using ClubPenguin.Net.Domain.Scene;
using Disney.Kelowna.Common.DataModel;
using System.Collections.Generic;

namespace ClubPenguin.Net
{
	public interface IIglooService : INetworkService
	{
		void CreateIglooLayout(MutableSceneLayout sceneLayout, IIglooCreateErrorHandler errorHandler = null);

		void UpdateIglooLayout(long layoutId, MutableSceneLayout sceneLayout, IIglooUpdateLayoutErrorHandler errorHandler = null);

		void DeleteIglooLayout(long layoutId, IIglooDeleteLayoutErrorHandler errorHandler = null);

		void GetIglooLayout(long layoutId);

		void GetFriendsIgloos(string language);

		void GetPopularIgloos(string language);

		void GetIglooPopulationsByZoneIds(string language, IList<ZoneId> zoneIds, List<DataEntityHandle> handles);

		void GetActiveIglooLayout(string iglooId);

		void UpdateIglooData(IglooVisibility? visibility, long? activeLayoutId, IIglooUpdateDataErrorHandler errorHandler = null);

		void GetDecorations();

		void PurchaseDecoration(int decorationDefinitionId, DecorationType type, int? count, IIglooCatalogPurchaseErrorHandler errorHandler);

		void UpdateAndPublish(long layoutId, IglooVisibility? visibility, MutableSceneLayout sceneLayout, IIglooUpdateAndPublishErrorHandler errorHandler);

		void QA_AddDecorations(int decorationDefinitionId, DecorationType type, int count);

		void QA_DeleteAllIgloos();

		void QA_DeleteAllDecorations();
	}
}
