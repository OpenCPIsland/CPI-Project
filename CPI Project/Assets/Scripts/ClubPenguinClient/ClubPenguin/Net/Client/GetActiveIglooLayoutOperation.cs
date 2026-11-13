using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain.Scene;
using ClubPenguin.Net.Offline;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;

namespace ClubPenguin.Net.Client
{
	[HttpPath("cp-api-base-uri", "/igloo/v1/iglooId/layout/active")]
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	[HttpPOST]
	[HttpAccept("application/json")]
	[RequestQueue("IglooLayout")]
	public class GetActiveIglooLayoutOperation : CPAPIHttpOperation
	{
		[HttpRequestTextBody]
		public string IglooId;

		[HttpResponseJsonBody]
		public SceneLayout ResponseBody;

		public GetActiveIglooLayoutOperation(string iglooId)
		{
			IglooId = iglooId;
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			ResponseBody = ReadActiveLayout(offlineDatabase);
		}

		// Every save lands in SceneLayoutEntity, while the copy in IglooEntity is only
		// refreshed when the igloo metadata changes - so reading that copy handed the
		// player a stale layout, and the next autosave wrote it back over the fresh one.
		// The copy is still the fallback: it is all a player has if the id is unknown.
		public static SceneLayout ReadActiveLayout(OfflineDatabase offlineDatabase)
		{
			IglooEntity iglooEntity = offlineDatabase.Read<IglooEntity>();
			if (iglooEntity.Data == null)
			{
				return null;
			}
			if (iglooEntity.Data.activeLayoutId.HasValue)
			{
				SceneLayoutEntity sceneLayoutEntity = offlineDatabase.Read<SceneLayoutEntity>();
				if (sceneLayoutEntity.Layouts != null)
				{
					SavedSceneLayout savedSceneLayout = sceneLayoutEntity[iglooEntity.Data.activeLayoutId.Value];
					if (savedSceneLayout != null)
					{
						return savedSceneLayout;
					}
				}
			}
			return iglooEntity.Data.activeLayout;
		}
	}
}
