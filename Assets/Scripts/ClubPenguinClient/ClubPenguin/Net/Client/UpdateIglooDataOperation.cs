using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using ClubPenguin.Net.Domain.Igloo;
using ClubPenguin.Net.Offline;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;

namespace ClubPenguin.Net.Client
{
	[HttpPUT]
	[RequestQueue("IglooLayout")]
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	[HttpPath("cp-api-base-uri", "/igloo/v1")]
	[HttpContentType("application/json")]
	[HttpAccept("application/json")]
	public class UpdateIglooDataOperation : CPAPIHttpOperation
	{
		[HttpRequestJsonBody]
		public MutableIglooData RequestBody;

		[HttpResponseJsonBody]
		public SignedResponse<IglooData> SignedResponseBody;

		public UpdateIglooDataOperation(MutableIglooData update)
		{
			RequestBody = update;
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			IglooEntity value = offlineDatabase.Read<IglooEntity>();
			if (RequestBody.visibility.HasValue)
			{
				value.Data.visibility = RequestBody.visibility;
			}
			if (RequestBody.activeLayoutId.HasValue)
			{
				SceneLayoutEntity sceneLayoutEntity = offlineDatabase.Read<SceneLayoutEntity>();
				if (sceneLayoutEntity[RequestBody.activeLayoutId.Value] != null)
				{
					value.Data.activeLayout = sceneLayoutEntity[RequestBody.activeLayoutId.Value];
					value.Data.activeLayoutId = RequestBody.activeLayoutId;
				}
			}
			offlineDatabase.Write(value);
			SignedResponseBody = new SignedResponse<IglooData>
			{
				Data = value.Data
			};
		}

		protected override void SetOfflineData(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			IglooEntity value = offlineDatabase.Read<IglooEntity>();
			value.Data = SignedResponseBody.Data;
			offlineDatabase.Write(value);
		}
	}
}
