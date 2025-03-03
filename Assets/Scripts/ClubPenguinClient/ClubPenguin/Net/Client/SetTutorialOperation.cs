using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using ClubPenguin.Net.Offline;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;
using System.Collections;
using System.Collections.Generic;

namespace ClubPenguin.Net.Client
{
	[HttpBasicAuthorization("cp-api-username", "cp-api-password")]
	[HttpPath("cp-api-base-uri", "/tutorial/v1/tutorial")]
	[HttpContentType("application/json")]
	[HttpAccept("application/json")]
	[HttpPOST]
	public class SetTutorialOperation : CPAPIHttpOperation
	{
		[HttpRequestJsonBody]
		public Tutorial Tutorial;

		[HttpResponseJsonBody]
		public TutorialResponse TutorialResponse;

		public SetTutorialOperation(Tutorial tutorial)
		{
			Tutorial = tutorial;
		}

		protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			TutorialData value = offlineDatabase.Read<TutorialData>();
			byte[] array = new byte[value.Bytes.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = (byte)value.Bytes[i];
			}
			BitArray bitArray = new BitArray(array);
			bitArray.Set(Tutorial.tutorialID, Tutorial.isComplete);
			bitArray.CopyTo(array, 0);
			for (int i = 0; i < array.Length; i++)
			{
				value.Bytes[i] = (sbyte)array[i];
			}
			offlineDatabase.Write(value);
			TutorialResponse = new TutorialResponse
			{
				tutorialBytes = new List<sbyte>(value.Bytes)
			};
		}

		protected override void SetOfflineData(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
			TutorialData value = offlineDatabase.Read<TutorialData>();
			for (int i = 0; i < value.Bytes.Length; i++)
			{
				sbyte b = 0;
				if (i < TutorialResponse.tutorialBytes.Count)
				{
					b = TutorialResponse.tutorialBytes[i];
				}
				value.Bytes[i] = b;
			}
			offlineDatabase.Write(value);
		}
	}
}
