using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using ClubPenguin.Net.Offline;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;
using System;
using System.Collections.Generic;

namespace ClubPenguin.Net.Client
{
    [HttpContentType("application/json")]
    [HttpAccept("application/json")]
    [HttpPath("cp-api-base-uri", "/catalog/v1/clothing/submit")]
    [HttpBasicAuthorization("cp-api-username", "cp-api-password")]
    [HttpPOST]
    public class ItemSubmissionOperation : CPAPIHttpOperation
    {
        [HttpRequestJsonBody]
        public ItemSubmissionRequest SubmissionRequest;

        [HttpResponseJsonBody]
        public ItemSubmissionResponse Response;

        public ItemSubmissionOperation(long scheduledThemeChallengeId, CustomEquipment equipment)
        {
            SubmissionRequest = default(ItemSubmissionRequest);
            SubmissionRequest.scheduledThemeChallengeId = scheduledThemeChallengeId;
            SubmissionRequest.equipment = default(CatalogSubmissionEquipment);
            SubmissionRequest.equipment.definitionId = equipment.definitionId;
            SubmissionRequest.equipment.parts = equipment.parts;
        }

        protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
        {
            ClothingCatalogOfflineData data = OfflineDatabase.Read<ClothingCatalogOfflineData>("clothing_catalog_offline");
            bool changed = ClothingCatalogOfflineData.EnsureSchema(ref data);

            if (data.NextCatalogItemId <= 0L)
                data.NextCatalogItemId = 1000L;

            long catalogItemId = data.NextCatalogItemId++;
            long nowMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            RegistrationProfile rp = offlineDatabase.Read<RegistrationProfile>();
            string creator = rp.displayName;
            if (string.IsNullOrEmpty(creator))
                creator = rp.userName ?? "";

            CustomEquipment eq = default(CustomEquipment);
            eq.equipmentId = 0L;
            eq.dateTimeCreated = nowMs;
            eq.definitionId = SubmissionRequest.equipment.definitionId;
            eq.parts = SubmissionRequest.equipment.parts;

            CatalogItemData item = default(CatalogItemData);
            item.clothingCatalogItemId = catalogItemId;
            item.creatorName = creator;
            item.cost = offlineDefinitions.GetEquipmentTemplateDefinitionCost(SubmissionRequest.equipment.definitionId);
            item.numberSold = 0L;
            item.equipment = eq;

            data.MarketplaceItems.Add(item);

            if (!data.SubmittedItemByChallenge.ContainsKey(SubmissionRequest.scheduledThemeChallengeId))
            {
                data.SubmittedItemByChallenge[SubmissionRequest.scheduledThemeChallengeId] = new List<long>();
            }
            data.SubmittedItemByChallenge[SubmissionRequest.scheduledThemeChallengeId].Add(catalogItemId);

            string userChallengeKey = rp.userName + ":" + SubmissionRequest.scheduledThemeChallengeId;
            data.UserCurrentSubmissionByChallenge[userChallengeKey] = catalogItemId;

            data.ScheduledThemeChallengeIdByCatalogItemId[catalogItemId] = SubmissionRequest.scheduledThemeChallengeId;

            ClubPenguin.Net.Offline.PlayerAssets pa = offlineDatabase.Read<ClubPenguin.Net.Offline.PlayerAssets>();
            pa.Assets.coins += (int)data.SubmissionRewardCoins;
            offlineDatabase.Write(pa);

            OfflineDatabase.Write(data, "clothing_catalog_offline");

            Response = new ItemSubmissionResponse();
            Response.clothingCatalogItemId = catalogItemId;
            Response.newCoinTotal = pa.Assets.coins;
        }
    }
}
