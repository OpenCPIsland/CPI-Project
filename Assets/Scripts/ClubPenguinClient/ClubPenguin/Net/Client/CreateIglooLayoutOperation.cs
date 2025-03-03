using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain.Scene;
using ClubPenguin.Net.Offline;
using Disney.Manimal.Common.Util;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;
using System;
using System.Collections.Generic;

namespace ClubPenguin.Net.Client
{
	[HttpPath("cp-api-base-uri", "/igloo/v1/layout")]
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	[HttpPOST]
	[HttpContentType("application/json")]
	[HttpAccept("application/json")]
	[RequestQueue("IglooLayout")]
	public class CreateIglooLayoutOperation : CPAPIHttpOperation
	{
		[HttpRequestJsonBody]
		public MutableSceneLayout RequestBody;

		[HttpResponseJsonBody]
		public SavedSceneLayout ResponseBody;

		public CreateIglooLayoutOperation(MutableSceneLayout sceneLayout)
		{
			RequestBody = sceneLayout;
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			SceneLayoutEntity value = offlineDatabase.Read<SceneLayoutEntity>();
			int count = value.Layouts.Count;
			Random random = new Random();
			byte[] array = new byte[8];
			random.NextBytes(array);
			SavedSceneLayout savedSceneLayout = new SavedSceneLayout();
			savedSceneLayout.createdDate = DateTime.UtcNow.GetTimeInMilliseconds();
			savedSceneLayout.decorationsLayout = RequestBody.decorationsLayout;
			savedSceneLayout.extraInfo = RequestBody.extraInfo;
			savedSceneLayout.lastModifiedDate = DateTime.UtcNow.GetTimeInMilliseconds();
			savedSceneLayout.layoutId = BitConverter.ToInt64(array, 0);
			savedSceneLayout.lightingId = RequestBody.lightingId;
			savedSceneLayout.memberOnly = true;
			savedSceneLayout.musicId = RequestBody.musicId;
			savedSceneLayout.name = RequestBody.name;
			savedSceneLayout.zoneId = RequestBody.zoneId;
			SavedSceneLayout savedSceneLayout2 = savedSceneLayout;
			if (savedSceneLayout2.decorationsLayout == null)
			{
				savedSceneLayout2.decorationsLayout = new List<DecorationLayout>();
			}
			if (savedSceneLayout2.extraInfo == null)
			{
				savedSceneLayout2.extraInfo = new Dictionary<string, string>();
			}
			if (count == 0)
			{
				IglooEntity value2 = offlineDatabase.Read<IglooEntity>();
				value2.Data.activeLayout = savedSceneLayout2;
				value2.Data.activeLayoutId = savedSceneLayout2.layoutId;
				offlineDatabase.Write(value2);
			}
			value.Layouts.Add(savedSceneLayout2);
			offlineDatabase.Write(value);
			ResponseBody = savedSceneLayout2;
		}

		protected override void SetOfflineData(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			SceneLayoutEntity value = offlineDatabase.Read<SceneLayoutEntity>();
			if (value.Layouts.Count == 0)
			{
				IglooEntity value2 = offlineDatabase.Read<IglooEntity>();
				value2.Data.activeLayout = ResponseBody;
				value2.Data.activeLayoutId = ResponseBody.layoutId;
				offlineDatabase.Write(value2);
			}
			value.Layouts.Add(ResponseBody);
			offlineDatabase.Write(value);
		}
	}
}
