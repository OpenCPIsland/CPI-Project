using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain.Scene;
using ClubPenguin.Net.Offline;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;

namespace ClubPenguin.Net.Client
{
	[HttpPath("cp-api-base-uri", "/igloo/v1/layout/{$sceneLayoutId}")]
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	[HttpGET]
	[HttpAccept("application/json")]
	[RequestQueue("IglooLayout")]
	public class GetIglooLayoutOperation : CPAPIHttpOperation
	{
		[HttpUriSegment("sceneLayoutId")]
		public long SceneLayoutId;

		[HttpResponseJsonBody]
		public SavedSceneLayout ResponseBody;

		public GetIglooLayoutOperation(long sceneLayoutId)
		{
			SceneLayoutId = sceneLayoutId;
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			ResponseBody = offlineDatabase.Read<SceneLayoutEntity>()[SceneLayoutId];
		}

		protected override void SetOfflineData(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			SceneLayoutEntity value = offlineDatabase.Read<SceneLayoutEntity>();
			for (int i = 0; i < value.Layouts.Count; i++)
			{
				if (value.Layouts[i].layoutId == SceneLayoutId)
				{
					value.Layouts.RemoveAt(i);
					break;
				}
			}
			value.Layouts.Add(ResponseBody);
			offlineDatabase.Write(value);
		}
	}
}
