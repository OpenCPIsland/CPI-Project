using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Domain;
using ClubPenguin.Net.Offline;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.mappers;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace ClubPenguin.Net.Client
{
    [HttpPath("cp-api-base-uri", "/catalog/v1/clothing/user/stats")]
    [HttpBasicAuthorization("cp-api-username", "cp-api-password")]
    [HttpGET]
    [HttpAccept("application/json")]
    public class GetUserStatsOperation : CPAPIHttpOperation
    {
        [HttpResponseJsonBody]
        public UserStatsResponse Response;

        protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
        {
            ClothingCatalogOfflineData data = OfflineDatabase.Read<ClothingCatalogOfflineData>("clothing_catalog_offline");
            ClothingCatalogOfflineData.EnsureSchema(ref data);

            RegistrationProfile rp = offlineDatabase.Read<RegistrationProfile>();
            string currentUsername = rp.userName ?? "";

            List<CatalogItemData> currentItems = new List<CatalogItemData>();
            if (data.MarketplaceItems != null && data.MarketplaceItems.Count > 0)
            {
                int today = ClothingCatalogOfflineData.GetDayStampUtc();

                long currentChallengeId = data.CurrentScheduledThemeChallengeId;
                if (data.CurrentDayStamp != today || currentChallengeId == 0L)
                {
                    currentChallengeId = FindScheduleIdForDayStamp(today);
                    data.CurrentDayStamp = today;
                    data.CurrentScheduledThemeChallengeId = currentChallengeId;
                    OfflineDatabase.Write(data, "clothing_catalog_offline");
                }

                HashSet<long> playerSubmittedIds = new HashSet<long>();
                if (data.UserCurrentSubmissionByChallenge != null)
                {
                    long cutoffMs = new DateTimeOffset(DateTime.UtcNow.Date.AddDays(-6), TimeSpan.Zero).ToUnixTimeMilliseconds();

                    string userPrefix = currentUsername + ":";
                    foreach (KeyValuePair<string, long> kvp in data.UserCurrentSubmissionByChallenge)
                    {
                        if (kvp.Key.StartsWith(userPrefix, StringComparison.Ordinal))
                            playerSubmittedIds.Add(kvp.Value);
                    }

                    for (int i = 0; i < data.MarketplaceItems.Count; i++)
                    {
                        CatalogItemData it = data.MarketplaceItems[i];
                        if (playerSubmittedIds.Contains(it.clothingCatalogItemId) &&
                            it.equipment.dateTimeCreated >= cutoffMs)
                        {
                            currentItems.Add(it);
                        }
                    }
                }
            }

            CatalogItemData mostPopular = default(CatalogItemData);
            bool foundPopular = false;

            if (data.MarketplaceItems != null && data.MarketplaceItems.Count > 0)
            {
                CatalogItemData best = data.MarketplaceItems[0];
                for (int i = 1; i < data.MarketplaceItems.Count; i++)
                {
                    CatalogItemData c = data.MarketplaceItems[i];
                    if (c.numberSold > best.numberSold)
                        best = c;
                }
                mostPopular = best;
                foundPopular = true;
            }

            if (!foundPopular)
            {
                mostPopular = default(CatalogItemData);
                mostPopular.creatorName = "";
                mostPopular.cost = 0L;
                mostPopular.numberSold = 0L;
                mostPopular.clothingCatalogItemId = 0L;
                mostPopular.equipment = default(CustomEquipment);
            }

            Response = default(UserStatsResponse);
            Response.totalItemsSold = data.TotalItemsSold;
            Response.totalItemsPurchased = data.TotalItemsPurchased;
            Response.mostPopularItem = mostPopular;
            Response.currentItems = currentItems;
        }

        private static long FindScheduleIdForDayStamp(int dayStampUtc)
        {
            int year = dayStampUtc / 10000;
            int month = (dayStampUtc / 100) % 100;
            int day = dayStampUtc % 100;

            Type scheduleType =
                FindType("ClubPenguin.CatalogThemeScheduleDefinition") ??
                FindType("ClubPenguin.CatalogThemeScheduleDefinitionKey") ??
                FindType("ClubPenguin.CatalogThemeScheduleDefinitionData");

            if (scheduleType != null)
            {
                UnityEngine.Object[] objs = Resources.FindObjectsOfTypeAll(scheduleType);
                if (objs != null && objs.Length > 0)
                {
                    for (int i = 0; i < objs.Length; i++)
                    {
                        if (objs[i] == null)
                            continue;

                        int y = TryReadIntMember(objs[i], "Year");
                        int m = TryReadIntMember(objs[i], "Month");
                        int d = TryReadIntMember(objs[i], "Day");

                        if (y == year && m == month && d == day)
                        {
                            long id = TryReadLongMember(objs[i], "Id");
                            if (id > 0L)
                                return id;
                        }
                    }
                }
            }

            return ClothingCatalogOfflineData.BuildChallengeIdFromDayStamp(dayStampUtc);
        }

        private static Type FindType(string fullName)
        {
            Assembly[] asms = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < asms.Length; i++)
            {
                try
                {
                    Type t = asms[i].GetType(fullName, false);
                    if (t != null)
                        return t;
                }
                catch { }
            }
            return null;
        }

        private static long TryReadLongMember(object obj, string name)
        {
            try
            {
                Type t = obj.GetType();

                FieldInfo f = t.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (f != null)
                {
                    object v = f.GetValue(obj);
                    return ConvertToLong(v);
                }

                PropertyInfo p = t.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (p != null && p.CanRead)
                {
                    object v2 = p.GetValue(obj, null);
                    return ConvertToLong(v2);
                }
            }
            catch { }

            return 0L;
        }

        private static int TryReadIntMember(object obj, string name)
        {
            try
            {
                Type t = obj.GetType();

                FieldInfo f = t.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (f != null)
                {
                    object v = f.GetValue(obj);
                    return ConvertToInt(v);
                }

                PropertyInfo p = t.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (p != null && p.CanRead)
                {
                    object v2 = p.GetValue(obj, null);
                    return ConvertToInt(v2);
                }
            }
            catch { }

            return 0;
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

        private static int ConvertToInt(object v)
        {
            if (v == null)
                return 0;

            try
            {
                if (v is int) return (int)v;
                if (v is long) return (int)(long)v;
                if (v is uint) return (int)(uint)v;
                if (v is short) return (short)v;
                if (v is ushort) return (ushort)v;
                if (v is byte) return (byte)v;
                if (v is sbyte) return (sbyte)v;
                if (v is string)
                {
                    int parsed;
                    if (int.TryParse((string)v, out parsed))
                        return parsed;
                }
                return System.Convert.ToInt32(v);
            }
            catch
            {
                return 0;
            }
        }
    }
}