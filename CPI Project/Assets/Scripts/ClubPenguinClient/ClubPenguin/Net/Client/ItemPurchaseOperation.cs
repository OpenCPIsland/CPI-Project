using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using ClubPenguin.Net.Offline;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;
using System;

namespace ClubPenguin.Net.Client
{
    [HttpAccept("application/json")]
    [HttpBasicAuthorization("cp-api-username", "cp-api-password")]
    [HttpPOST]
    [HttpContentType("application/json")]
    [HttpPath("cp-api-base-uri", "/catalog/v1/clothing/purchase")]
    public class ItemPurchaseOperation : CPAPIHttpOperation
    {
        [HttpRequestJsonBody]
        public ItemPurchaseRequest PurchaseRequest;

        [HttpResponseJsonBody]
        public ItemPurchaseResponse Response;

        public ItemPurchaseOperation(long clothingCatalogItemId)
        {
            PurchaseRequest = default(ItemPurchaseRequest);
            PurchaseRequest.clothingCatalogItemId = clothingCatalogItemId;
        }

        protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
        {
            ClothingCatalogOfflineData data = OfflineDatabase.Read<ClothingCatalogOfflineData>("clothing_catalog_offline");

            if (data.MarketplaceItems == null)
                data.MarketplaceItems = new System.Collections.Generic.List<CatalogItemData>();

            int idx = -1;
            for (int i = 0; i < data.MarketplaceItems.Count; i++)
            {
                if (data.MarketplaceItems[i].clothingCatalogItemId == PurchaseRequest.clothingCatalogItemId)
                {
                    idx = i;
                    break;
                }
            }

            ClubPenguin.Net.Offline.PlayerAssets pa = offlineDatabase.Read<ClubPenguin.Net.Offline.PlayerAssets>();

            if (LanCatalog.Active)
            {
                CatalogItemData? found = LanCatalog.Find(PurchaseRequest.clothingCatalogItemId);
                long lanCost = found.HasValue ? Math.Max(0L, found.Value.cost) : 0L;
                if (!found.HasValue || pa.Assets.coins < (int)lanCost)
                {
                    Response = default(ItemPurchaseResponse);
                    Response.equipmentId = 0L;
                    Response.newCoinTotal = pa.Assets.coins;
                    return;
                }
                pa.Assets.coins -= (int)lanCost;
                data.TotalItemsPurchased += 1L;
                LanCatalog.Purchase(PurchaseRequest.clothingCatalogItemId);
                long lanEquipmentId = LanCatalog.NewItemId();
                CustomEquipment lanPurchased = found.Value.equipment;
                lanPurchased.equipmentId = lanEquipmentId;
                lanPurchased.dateTimeCreated = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                CustomEquipmentCollection lanCollection = offlineDatabase.Read<CustomEquipmentCollection>();
                if (lanCollection.Equipment == null)
                    lanCollection.Equipment = new System.Collections.Generic.List<CustomEquipment>();
                lanCollection.Equipment.Add(lanPurchased);
                offlineDatabase.Write(lanCollection);
                offlineDatabase.Write(pa);
                OfflineDatabase.Write(data, "clothing_catalog_offline");
                Response = default(ItemPurchaseResponse);
                Response.equipmentId = lanEquipmentId;
                Response.newCoinTotal = pa.Assets.coins;
                return;
            }

            if (idx < 0)
            {
                Response = default(ItemPurchaseResponse);
                Response.equipmentId = 0L;
                Response.newCoinTotal = pa.Assets.coins;
                return;
            }

            CatalogItemData item = data.MarketplaceItems[idx];
            long cost = item.cost;
            if (cost < 0L) cost = 0L;

            if (pa.Assets.coins < (int)cost)
            {
                Response = default(ItemPurchaseResponse);
                Response.equipmentId = 0L;
                Response.newCoinTotal = pa.Assets.coins;
                return;
            }

            pa.Assets.coins -= (int)cost;
            data.TotalItemsPurchased += 1L;

            item.numberSold += 1L;
            data.TotalItemsSold += 1L;
            data.MarketplaceItems[idx] = item;

            Random random = new Random();
            byte[] bytes = new byte[8];
            random.NextBytes(bytes);
            long newEquipmentId = BitConverter.ToInt64(bytes, 0);

            CustomEquipment purchased = item.equipment;
            purchased.equipmentId = newEquipmentId;
            purchased.dateTimeCreated = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            CustomEquipmentCollection eqCol = offlineDatabase.Read<CustomEquipmentCollection>();
            if (eqCol.Equipment == null)
                eqCol.Equipment = new System.Collections.Generic.List<CustomEquipment>();
            eqCol.Equipment.Add(purchased);

            offlineDatabase.Write(eqCol);
            offlineDatabase.Write(pa);
            OfflineDatabase.Write(data, "clothing_catalog_offline");

            Response = default(ItemPurchaseResponse);
            Response.equipmentId = newEquipmentId;
            Response.newCoinTotal = pa.Assets.coins;
        }
    }
}
