using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain.Scene;
using ClubPenguin.Net.Offline;
using Disney.Manimal.Common.Util;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;
using System;

namespace ClubPenguin.Net.Client
{
	[RequestQueue("IglooLayout")]
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	[HttpPUT]
	[HttpPath("cp-api-base-uri", "/igloo/v1/layout/{$sceneLayoutId}")]
	[HttpContentType("application/json")]
	[HttpAccept("application/json")]
	public class UpdateIglooLayoutOperation : CPAPIHttpOperation
	{
		[HttpUriSegment("sceneLayoutId")]
		public long SceneLayoutId;

		[HttpRequestJsonBody]
		public MutableSceneLayout RequestBody;

		[HttpResponseJsonBody]
		public SavedSceneLayout ResponseBody;

		public UpdateIglooLayoutOperation(long sceneLayoutId, MutableSceneLayout sceneLayout)
		{
			SceneLayoutId = sceneLayoutId;
			RequestBody = sceneLayout;
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			SceneLayoutEntity value = offlineDatabase.Read<SceneLayoutEntity>();
			SceneLayout sceneLayout = value[SceneLayoutId];
			sceneLayout.lastModifiedDate = DateTime.UtcNow.GetTimeInMilliseconds();
			if (RequestBody.zoneId != null)
			{
				sceneLayout.zoneId = RequestBody.zoneId;
			}
			if (RequestBody.name != null)
			{
				sceneLayout.name = RequestBody.name;
			}
			sceneLayout.lightingId = RequestBody.lightingId;
			sceneLayout.musicId = RequestBody.musicId;
			if (RequestBody.decorationsLayout != null)
			{
				sceneLayout.decorationsLayout = RequestBody.decorationsLayout;
			}
			if (RequestBody.extraInfo != null)
			{
				sceneLayout.extraInfo = RequestBody.extraInfo;
			}
			offlineDatabase.Write(value);
			ResponseBody = SavedSceneLayout.FromSceneLayout(sceneLayout, SceneLayoutId);
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
