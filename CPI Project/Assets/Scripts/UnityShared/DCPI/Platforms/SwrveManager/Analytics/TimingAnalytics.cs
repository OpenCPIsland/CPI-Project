using System.Collections.Generic;

namespace DCPI.Platforms.SwrveManager.Analytics
{
	public class TimingAnalytics : GameAnalytics
	{
		private int _elapsedTime;

		private string _context = null;

		private int _stepNumber;

		private string _stepName = null;

		private IDictionary<string, object> _custom = null;

		public int ElapsedTime
		{
			get
			{
				return _elapsedTime;
			}
		}

		public string Context
		{
			get
			{
				return _context;
			}
		}

		public int StepNumber
		{
			get
			{
				return _stepNumber;
			}
		}

		public string StepName
		{
			get
			{
				return _stepName;
			}
		}

		public IDictionary<string, object> Custom
		{
			get
			{
				return _custom;
			}
		}

		public TimingAnalytics(int elapsedTime, string context)
		{
			InitTimingAnalytics(elapsedTime, context, -1, null, null);
		}

		public TimingAnalytics(int elapsedTime, string context, int stepNumber)
		{
			InitTimingAnalytics(elapsedTime, context, stepNumber, null, null);
		}

		public TimingAnalytics(int elapsedTime, string context, int stepNumber, string stepName)
		{
			InitTimingAnalytics(elapsedTime, context, stepNumber, stepName, null);
		}

		public TimingAnalytics(int elapsedTime, string context, int stepNumber, string stepName, IDictionary<string, object> custom)
		{
			InitTimingAnalytics(elapsedTime, context, stepNumber, stepName, custom);
		}

		private void InitTimingAnalytics(int elapsedTime, string context, int stepNumber, string stepName, IDictionary<string, object> custom)
		{
			_elapsedTime = elapsedTime;
			_context = context;
			_stepNumber = stepNumber;
			_stepName = stepName;
			if (custom != null)
			{
				_custom = new Dictionary<string, object>(custom);
			}
		}

		public override string GetSwrveEvent()
		{
			return "timing";
		}

		public override Dictionary<string, object> Serialize()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["elapsed_time"] = _elapsedTime;
			dictionary["context"] = _context;
			if (_stepNumber > 0)
			{
				dictionary["step_number"] = _stepNumber;
			}
			if (!string.IsNullOrEmpty(_stepName))
			{
				dictionary["step_name"] = _stepName;
			}
			if (_custom != null)
			{
				foreach (KeyValuePair<string, object> item in _custom)
				{
					dictionary[item.Key] = item.Value;
				}
			}
			return dictionary;
		}
	}
}
