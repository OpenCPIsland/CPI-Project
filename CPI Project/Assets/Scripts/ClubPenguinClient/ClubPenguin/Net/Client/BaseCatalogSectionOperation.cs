using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using ClubPenguin.Net.Offline;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace ClubPenguin.Net.Client
{
    [HttpContentType("application/json")]
    [HttpPath("cp-api-base-uri", "/catalog/v1/clothing/items/{$catalogSection}")]
    [HttpAccept("application/json")]
    [HttpBasicAuthorization("cp-api-username", "cp-api-password")]
    [HttpPOST]
    public class BaseCatalogSectionOperation<T> : CPAPIHttpOperation
    {
        [HttpUriSegment("catalogSection")]
        public string CatalogSection;

        [HttpRequestJsonBody]
        public T SectionRequest;

        [HttpResponseJsonBody]
        public CatalogSectionResponse Response;

        public BaseCatalogSectionOperation(T sectionRequest)
        {
            SectionRequest = sectionRequest;
        }

        protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
        {
            ClothingCatalogOfflineData data = OfflineDatabase.Read<ClothingCatalogOfflineData>("clothing_catalog_offline");
            ClothingCatalogOfflineData.EnsureSchema(ref data);

            string cursor = GetCursorValue(SectionRequest);
            int startIndex = ParseCursor(cursor);

            long requestedChallengeId = GetScheduledThemeChallengeIdValue(SectionRequest);

            List<CatalogItemData> items = new List<CatalogItemData>();

            if (requestedChallengeId > 0L && data.MarketplaceItems != null && data.MarketplaceItems.Count > 0)
            {
                for (int i = 0; i < data.MarketplaceItems.Count; i++)
                {
                    CatalogItemData it = data.MarketplaceItems[i];
                    if (data.ScheduledThemeChallengeIdByCatalogItemId != null &&
                        data.ScheduledThemeChallengeIdByCatalogItemId.TryGetValue(it.clothingCatalogItemId, out long itemChallengeId) &&
                        itemChallengeId == requestedChallengeId)
                    {
                        items.Add(it);
                    }
                }
            }

            string section = CatalogSection ?? "";
            if (section.Equals("popular", StringComparison.OrdinalIgnoreCase))
            {
                items.Sort((a, b) => b.numberSold.CompareTo(a.numberSold));
            }
            else
            {
                items.Sort((a, b) => b.equipment.dateTimeCreated.CompareTo(a.equipment.dateTimeCreated));
            }

            const int pageSize = 20;

            if (startIndex < 0) startIndex = 0;
            if (startIndex > items.Count) startIndex = items.Count;

            int count = items.Count - startIndex;
            if (count > pageSize) count = pageSize;

            List<CatalogItemData> page = new List<CatalogItemData>();
            for (int i = 0; i < count; i++)
                page.Add(items[startIndex + i]);

            int nextIndex = startIndex + count;
            string nextCursor = (nextIndex < items.Count) ? nextIndex.ToString(CultureInfo.InvariantCulture) : "";

            Response = default(CatalogSectionResponse);
            Response.cursor = nextCursor;
            Response.items = page;

            OfflineDatabase.Write(data, "clothing_catalog_offline");
        }

        private static int ParseCursor(string cursor)
        {
            if (string.IsNullOrEmpty(cursor))
                return 0;

            if (int.TryParse(cursor, NumberStyles.Integer, CultureInfo.InvariantCulture, out int v))
                return v;

            return 0;
        }

        private static string GetCursorValue(object req)
        {
            if (req == null)
                return "";

            Type t = req.GetType();
            FieldInfo f = t.GetField("cursor", BindingFlags.Public | BindingFlags.Instance);
            if (f != null && f.FieldType == typeof(string))
            {
                object val = f.GetValue(req);
                return (val as string) ?? "";
            }

            PropertyInfo p = t.GetProperty("cursor", BindingFlags.Public | BindingFlags.Instance);
            if (p != null && p.PropertyType == typeof(string))
            {
                object val2 = p.GetValue(req, null);
                return (val2 as string) ?? "";
            }

            return "";
        }

        private static long GetScheduledThemeChallengeIdValue(object req)
        {
            if (req == null)
                return 0L;

            Type t = req.GetType();

            FieldInfo f = t.GetField("scheduledThemeChallengeId", BindingFlags.Public | BindingFlags.Instance);
            if (f != null)
            {
                object v = f.GetValue(req);
                return ConvertToLong(v);
            }

            PropertyInfo p = t.GetProperty("scheduledThemeChallengeId", BindingFlags.Public | BindingFlags.Instance);
            if (p != null && p.CanRead)
            {
                object v2 = p.GetValue(req, null);
                return ConvertToLong(v2);
            }

            return 0L;
        }

        private static long ConvertToLong(object v)
        {
            if (v == null)
                return 0L;

            try
            {
                if (v is long) return (long)v;
                if (v is int) return (int)v;
                if (v is uint) return (uint)v;
                if (v is short) return (short)v;
                if (v is ushort) return (ushort)v;
                if (v is byte) return (byte)v;
                if (v is sbyte) return (sbyte)v;
                if (v is string)
                {
                    long parsed;
                    if (long.TryParse((string)v, out parsed))
                        return parsed;
                }
                return System.Convert.ToInt64(v);
            }
            catch
            {
                return 0L;
            }
        }
    }
}