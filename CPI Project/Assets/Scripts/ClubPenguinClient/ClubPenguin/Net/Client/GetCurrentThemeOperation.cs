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
    [HttpAccept("application/json")]
    [HttpPath("cp-api-base-uri", "/catalog/v1/clothing/themes/stats")]
    [HttpBasicAuthorization("cp-api-username", "cp-api-password")]
    [HttpGET]
    public class GetCurrentThemeOperation : CPAPIHttpOperation
    {
        [HttpResponseJsonBody]
        public CurrentThemeResponse Response;

        private static List<long> cachedScheduleIds;
        private static Dictionary<int, long> cachedScheduleIdByDayStampUtc;
        private static HashSet<long> cachedThemeDefinitionIds;

        protected override void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
        {
            ClothingCatalogOfflineData data = OfflineDatabase.Read<ClothingCatalogOfflineData>("clothing_catalog_offline");
            ClothingCatalogOfflineData.EnsureSchema(ref data);

            RegistrationProfile rp = offlineDatabase.Read<RegistrationProfile>();
            string currentUsername = rp.userName ?? "";

            EnsureScheduleCaches();

            DateTime utcToday = DateTime.UtcNow.Date;
            int todayStamp = ClothingCatalogOfflineData.GetDayStampUtc(utcToday);

            int[] dayStamps = new int[7];
            for (int i = 0; i < 7; i++)
            {
                DateTime day = utcToday.AddDays(-i);
                dayStamps[i] = ClothingCatalogOfflineData.GetDayStampUtc(day);
            }

            long[] scheduleIds = AssignScheduleIdsForWeek(dayStamps);

            List<CurrentThemeData> themes = new List<CurrentThemeData>(7);

            for (int i = 0; i < 7; i++)
            {
                long scheduleId = scheduleIds[i];

                long userSubmissionId = 0L;
                string userChallengeKey = currentUsername + ":" + scheduleId;
                if (data.UserCurrentSubmissionByChallenge != null && data.UserCurrentSubmissionByChallenge.TryGetValue(userChallengeKey, out long subId))
                    userSubmissionId = subId;

                CatalogItemData? mostPopular = FindMostPopularForChallenge(data, scheduleId);

                CurrentThemeData theme = default(CurrentThemeData);
                theme.scheduledThemeChallengeId = scheduleId;
                theme.userSubmissionClothingCatalogId = userSubmissionId;
                theme.mostPopularItem = mostPopular;
                theme.submissionRewardCoinAmount = data.SubmissionRewardCoins;
                themes.Add(theme);
            }

            data.CurrentDayStamp = todayStamp;
            if (themes.Count > 0)
                data.CurrentScheduledThemeChallengeId = themes[0].scheduledThemeChallengeId;

            OfflineDatabase.Write(data, "clothing_catalog_offline");

            Response = new CurrentThemeResponse
            {
                themes = themes
            };
        }

        private static long[] AssignScheduleIdsForWeek(int[] dayStamps)
        {
            long[] result = new long[dayStamps.Length];
            bool hasThemeDefs = cachedThemeDefinitionIds != null && cachedThemeDefinitionIds.Count > 0;

            List<long> pool = new List<long>();
            if (cachedScheduleIds != null)
            {
                for (int i = 0; i < cachedScheduleIds.Count; i++)
                {
                    long id = cachedScheduleIds[i];
                    if (id <= 0L)
                        continue;
                    if (hasThemeDefs && !cachedThemeDefinitionIds.Contains(id))
                        continue;
                    pool.Add(id);
                }
            }

            for (int i = 0; i < dayStamps.Length; i++)
            {
                int dayStamp = dayStamps[i];

                if (cachedScheduleIdByDayStampUtc != null &&
                    cachedScheduleIdByDayStampUtc.TryGetValue(dayStamp, out long explicitId) &&
                    explicitId > 0L &&
                    (!hasThemeDefs || cachedThemeDefinitionIds.Contains(explicitId)))
                {
                    result[i] = explicitId;
                    continue;
                }

                if (pool.Count == 0)
                    continue;

                int idx = (int)((uint)dayStamp % (uint)pool.Count);
                result[i] = pool[idx];
            }

            return result;
        }

        private static CatalogItemData? FindMostPopularForChallenge(ClothingCatalogOfflineData data, long scheduledThemeChallengeId)
        {
            if (data.MarketplaceItems == null || data.MarketplaceItems.Count == 0)
                return null;

            CatalogItemData best = default(CatalogItemData);
            bool found = false;

            for (int i = 0; i < data.MarketplaceItems.Count; i++)
            {
                CatalogItemData item = data.MarketplaceItems[i];
                if (data.ScheduledThemeChallengeIdByCatalogItemId != null &&
                    data.ScheduledThemeChallengeIdByCatalogItemId.TryGetValue(item.clothingCatalogItemId, out long itemChallengeId) &&
                    itemChallengeId == scheduledThemeChallengeId)
                {
                    if (!found || item.numberSold > best.numberSold)
                    {
                        best = item;
                        found = true;
                    }
                }
            }

            if (!found)
                return null;

            return best;
        }

        private static void EnsureScheduleCaches()
        {
            if (cachedScheduleIds != null && cachedScheduleIdByDayStampUtc != null &&
                cachedScheduleIdByDayStampUtc.Count > 0 && cachedThemeDefinitionIds != null)
                return;

            cachedScheduleIds = new List<long>();
            cachedScheduleIdByDayStampUtc = new Dictionary<int, long>();
            cachedThemeDefinitionIds = new HashSet<long>();

            Type scheduleType =
                FindType("ClubPenguin.CatalogThemeScheduleDefinition") ??
                FindType("ClubPenguin.CatalogThemeScheduleDefinitionKey") ??
                FindType("ClubPenguin.CatalogThemeScheduleDefinitionData") ??
                FindType("ClubPenguin.Net.Domain.CatalogThemeScheduleDefinition") ??
                FindType("ClubPenguin.Net.Domain.CatalogThemeScheduleDefinitionKey") ??
                FindType("ClubPenguin.Net.Domain.CatalogThemeScheduleDefinitionData");

            HashSet<long> seenScheduleIds = new HashSet<long>();

            if (scheduleType != null)
            {
                UnityEngine.Object[] objs = Resources.FindObjectsOfTypeAll(scheduleType);
                if (objs != null)
                {
                    for (int i = 0; i < objs.Length; i++)
                    {
                        if (objs[i] == null)
                            continue;

                        long id = TryReadLongMember(objs[i], "Id");
                        if (id == 0L) id = TryReadLongMember(objs[i], "id");
                        if (id == 0L) id = TryReadLongMember(objs[i], "ScheduleId");
                        if (id == 0L) id = TryReadLongMember(objs[i], "scheduleId");
                        if (id == 0L) id = TryReadLongMember(objs[i], "scheduledThemeChallengeId");
                        if (id == 0L) id = TryReadLongMember(objs[i], "ScheduledThemeChallengeId");

                        if (id > 0L && seenScheduleIds.Add(id))
                            cachedScheduleIds.Add(id);

                        int year = TryReadIntMember(objs[i], "Year");
                        if (year == 0) year = TryReadIntMember(objs[i], "year");
                        int month = TryReadIntMember(objs[i], "Month");
                        if (month == 0) month = TryReadIntMember(objs[i], "month");
                        int day = TryReadIntMember(objs[i], "Day");
                        if (day == 0) day = TryReadIntMember(objs[i], "day");

                        if (year > 0 && month > 0 && day > 0 && id > 0L)
                        {
                            int dayStamp = (year * 10000) + (month * 100) + day;
                            if (!cachedScheduleIdByDayStampUtc.ContainsKey(dayStamp))
                                cachedScheduleIdByDayStampUtc[dayStamp] = id;
                        }
                    }
                }
            }

            Assembly[] asms = AppDomain.CurrentDomain.GetAssemblies();
            for (int a = 0; a < asms.Length; a++)
            {
                try
                {
                    Type[] types = asms[a].GetTypes();
                    for (int t = 0; t < types.Length; t++)
                    {
                        string n = types[t].Name;
                        if (n.IndexOf("Theme", StringComparison.OrdinalIgnoreCase) < 0)
                            continue;
                        if (n.IndexOf("Schedule", StringComparison.OrdinalIgnoreCase) >= 0)
                            continue;
                        if (!typeof(UnityEngine.Object).IsAssignableFrom(types[t]))
                            continue;

                        try
                        {
                            UnityEngine.Object[] themeObjs = Resources.FindObjectsOfTypeAll(types[t]);
                            if (themeObjs == null)
                                continue;

                            for (int i = 0; i < themeObjs.Length; i++)
                            {
                                if (themeObjs[i] == null)
                                    continue;

                                long id = TryReadLongMember(themeObjs[i], "Id");
                                if (id == 0L) id = TryReadLongMember(themeObjs[i], "id");
                                if (id == 0L) id = TryReadLongMember(themeObjs[i], "ThemeId");
                                if (id == 0L) id = TryReadLongMember(themeObjs[i], "themeId");
                                if (id > 0L)
                                    cachedThemeDefinitionIds.Add(id);
                            }
                        }
                        catch { }
                    }
                }
                catch { }
            }
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