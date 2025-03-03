using SwrveUnity.Helpers;
using System.Collections.Generic;

namespace SwrveUnity.ResourceManager
{
	public class SwrveResourceManager
	{
		public Dictionary<string, SwrveResource> UserResources;

		public List<SwrveABTestDetails> ABTestDetails;

		public SwrveResourceManager()
		{
			UserResources = new Dictionary<string, SwrveResource>();
			ABTestDetails = new List<SwrveABTestDetails>();
		}

		public void SetResourcesFromJSON(Dictionary<string, Dictionary<string, string>> userResourcesJson)
		{
			Dictionary<string, SwrveResource> dictionary = new Dictionary<string, SwrveResource>();
			Dictionary<string, Dictionary<string, string>>.Enumerator enumerator = userResourcesJson.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<string, Dictionary<string, string>> current = enumerator.Current;
				dictionary[current.Key] = new SwrveResource(current.Value);
			}
			UserResources = dictionary;
		}

		public void SetABTestDetailsFromJSON(Dictionary<string, object> abTestDetailsJson)
		{
			List<SwrveABTestDetails> list = new List<SwrveABTestDetails>();
			Dictionary<string, object>.Enumerator enumerator = abTestDetailsJson.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<string, object> current = enumerator.Current;
				if (current.Value is Dictionary<string, object>)
				{
					Dictionary<string, object> dictionary = (Dictionary<string, object>)current.Value;
					string name = (string)dictionary["name"];
					int @int = MiniJsonHelper.GetInt(dictionary, "case_index", 0);
					SwrveABTestDetails item = new SwrveABTestDetails(current.Key, name, @int);
					list.Add(item);
				}
			}
			ABTestDetails = list;
		}

		public SwrveResource GetResource(string resourceId)
		{
			if (UserResources != null)
			{
				if (UserResources.ContainsKey(resourceId))
				{
					return UserResources[resourceId];
				}
			}
			else
			{
				SwrveLog.LogWarning(string.Format("SwrveResourceManager::GetResource('{0}'): Resources are not available yet.", resourceId));
			}
			return null;
		}

		public T GetResourceAttribute<T>(string resourceId, string attributeName, T defaultValue)
		{
			SwrveResource resource = GetResource(resourceId);
			if (resource != null)
			{
				return resource.GetAttribute(attributeName, defaultValue);
			}
			return defaultValue;
		}
	}
}
