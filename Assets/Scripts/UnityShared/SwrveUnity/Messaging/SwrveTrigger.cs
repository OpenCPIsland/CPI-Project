using SwrveUnityMiniJSON;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SwrveUnity.Messaging
{
	public class SwrveTrigger
	{
		private const string EVENT_NAME_KEY = "event_name";

		private const string CONDITIONS_KEY = "conditions";

		private string eventName;

		private SwrveConditions conditions;

		public string GetEventName()
		{
			return eventName;
		}

		public SwrveConditions GetConditions()
		{
			return conditions;
		}

		public bool CanTrigger(string eventName, IDictionary<string, string> payload)
		{
			return string.Equals(this.eventName, eventName, StringComparison.OrdinalIgnoreCase) && (conditions == null || conditions.Matches(payload));
		}

		public static SwrveTrigger LoadFromJson(object json)
		{
			IDictionary<string, object> dictionary = null;
			try
			{
				dictionary = (IDictionary<string, object>)json;
			}
			catch (Exception ex)
			{
				SwrveLog.LogError(string.Format("Invalid object passed in to LoadFromJson, expected Dictionary<string, object>, received {0}, exception: {1}", json, ex.Message));
				return null;
			}
			string value = null;
			SwrveConditions swrveConditions = null;
			try
			{
				value = (string)dictionary["event_name"];
				if (dictionary.ContainsKey("conditions"))
				{
					swrveConditions = SwrveConditions.LoadFromJson((IDictionary<string, object>)dictionary["conditions"], true);
				}
			}
			catch (Exception ex)
			{
				SwrveLog.LogError(string.Format("Error parsing a SwrveTrigger from json {0}, ex: {1}", dictionary, ex));
			}
			if (string.IsNullOrEmpty(value) || swrveConditions == null)
			{
				return null;
			}
			SwrveTrigger swrveTrigger = new SwrveTrigger();
			swrveTrigger.eventName = value;
			swrveTrigger.conditions = swrveConditions;
			return swrveTrigger;
		}

		public static IEnumerable<SwrveTrigger> LoadFromJson(List<object> triggers)
		{
			try
			{
				return from dict in triggers
					select LoadFromJson(dict) into dict
					where dict != null
					select dict;
			}
			catch (Exception arg)
			{
				SwrveLog.LogError(string.Format("Error creating a list of SwrveTriggers, ex: {0}", arg));
			}
			return null;
		}

		public static IEnumerable<SwrveTrigger> LoadFromJson(string json)
		{
			try
			{
				object obj = Json.Deserialize(json);
				return LoadFromJson((List<object>)obj);
			}
			catch (Exception arg)
			{
				SwrveLog.LogError(string.Format("Error parsing a SwrveTrigger from json {0}, ex: {1}", json, arg));
			}
			return null;
		}

		public override string ToString()
		{
			return string.Concat("Trigger{eventName='", eventName, '\'', ", conditions=", conditions, '}');
		}
	}
}
