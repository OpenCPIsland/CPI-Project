using System.Collections.Generic;

namespace DCPI.Platforms.SwrveManager.Analytics
{
	public class FunnelStepsAnalytics : GameAnalytics
	{
		private string _type;

		private int _stepNumber;

		private string _stepName;

		private string _message = string.Empty;

		private IDictionary<string, object> _custom = null;

		public string Type
		{
			get
			{
				return _type;
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

		public string Message
		{
			get
			{
				return _message;
			}
		}

		public IDictionary<string, object> Custom
		{
			get
			{
				return _custom;
			}
		}

		public FunnelStepsAnalytics(string type, int stepNumber, string stepName)
		{
			InitFunnelStepsAnalytics(type, stepNumber, stepName, null, null);
		}

		public FunnelStepsAnalytics(string type, int stepNumber, string stepName, string message)
		{
			InitFunnelStepsAnalytics(type, stepNumber, stepName, message, null);
		}

		public FunnelStepsAnalytics(string type, int stepNumber, string stepName, string message, IDictionary<string, object> custom)
		{
			InitFunnelStepsAnalytics(type, stepNumber, stepName, message, custom);
		}

		private void InitFunnelStepsAnalytics(string type, int stepNumber, string stepName, string message, IDictionary<string, object> custom)
		{
			_type = type;
			_stepNumber = stepNumber;
			_stepName = stepName;
			_message = message;
			if (custom != null)
			{
				_custom = new Dictionary<string, object>(custom);
			}
		}

		public override string GetSwrveEvent()
		{
			return "funnel." + _type + "." + ((_stepNumber < 10) ? ("0" + _stepNumber) : _stepNumber.ToString()) + "." + _stepName;
		}

		public override Dictionary<string, object> Serialize()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("type", _type);
			dictionary.Add("step_number", _stepNumber);
			dictionary.Add("step_name", _stepName);
			if (!string.IsNullOrEmpty(_message))
			{
				dictionary.Add("message", _message);
			}
			if (_custom != null)
			{
				foreach (KeyValuePair<string, object> item in _custom)
				{
					dictionary.Add(item.Key, item.Value);
				}
			}
			return dictionary;
		}
	}
}
