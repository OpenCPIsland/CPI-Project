using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Offline;
using hg.ApiWebKit.core.attributes;

namespace ClubPenguin.Net.Client
{
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	[RequestQueue("IglooLayout")]
	[HttpDELETE]
	[HttpPath("cp-api-base-uri", "/igloo/v1/layout/{$sceneLayoutId}")]
	public class DeleteIglooLayoutOperation : CPAPIHttpOperation
	{
		[HttpUriSegment("sceneLayoutId")]
		public long SceneLayoutId;

		public DeleteIglooLayoutOperation(long sceneLayoutId)
		{
			SceneLayoutId = sceneLayoutId;
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
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
			offlineDatabase.Write(value);
		}

		protected override void SetOfflineData(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			PerformOfflineAction(offlineDatabase, offlineDefinitions);
		}
	}
}
