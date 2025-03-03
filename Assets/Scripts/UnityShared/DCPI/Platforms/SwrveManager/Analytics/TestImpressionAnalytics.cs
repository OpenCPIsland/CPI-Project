using System.Collections.Generic;

namespace DCPI.Platforms.SwrveManager.Analytics
{
	public class TestImpressionAnalytics : GameAnalytics
	{
		private string _testName = string.Empty;

		private int _shardNumber;

		private bool _applied = false;

		public string TestName
		{
			get
			{
				return _testName;
			}
		}

		public int ShardNumber
		{
			get
			{
				return _shardNumber;
			}
		}

		public bool IsApplied
		{
			get
			{
				return _applied;
			}
		}

		public TestImpressionAnalytics(string testName, int shardNumber, bool isApplied)
		{
			_testName = testName;
			_shardNumber = shardNumber;
			_applied = isApplied;
		}

		public override string GetSwrveEvent()
		{
			return "test_impression";
		}

		public override Dictionary<string, object> Serialize()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["test_name"] = _testName;
			dictionary["shard_number"] = _shardNumber;
			dictionary["applied"] = (_applied ? 1 : 0);
			return dictionary;
		}
	}
}
