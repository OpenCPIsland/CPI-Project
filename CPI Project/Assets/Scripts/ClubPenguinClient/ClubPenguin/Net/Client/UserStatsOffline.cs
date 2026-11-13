using System;
using System.Collections.Generic;

[Serializable]
public class UserStatsOffline
{
    public Dictionary<long, List<long>> SubmissionsByThemeId = new Dictionary<long, List<long>>();

    public List<long> ThisWeekThemeIds = new List<long>();

    public long LastWeekStartUtcTicks;
    public int LastThemeIndex;
    public long LastSubmissionUtcTicks;

    public UserStatsOffline()
    {
    }
}
