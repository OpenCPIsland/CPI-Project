using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class UIUserNotificationCategory
{
	private const string IDENTIFIER_KEY = "identifier";

	private const string CONTEXT_ACTIONS_KEY = "contextActions";

	public string identifier;

	public List<UIUserNotificationAction> defaultContextActions;

	public List<UIUserNotificationAction> minimalContextActions;

	public Dictionary<string, object> toDict()
	{
		Dictionary<UIUserNotificationActionContext, List<UIUserNotificationAction>> dictionary = new Dictionary<UIUserNotificationActionContext, List<UIUserNotificationAction>>();
		if (0 < defaultContextActions.Count)
		{
			dictionary[UIUserNotificationActionContext.UIUserNotificationActionContextDefault] = defaultContextActions;
		}
		if (0 < minimalContextActions.Count)
		{
			dictionary[UIUserNotificationActionContext.UIUserNotificationActionContextMinimal] = minimalContextActions;
		}
		Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
		if (0 < dictionary.Keys.Count)
		{
			dictionary2["identifier"] = identifier;
			dictionary2["contextActions"] = dictionary.ToDictionary((KeyValuePair<UIUserNotificationActionContext, List<UIUserNotificationAction>> x) => (int)x.Key, (KeyValuePair<UIUserNotificationActionContext, List<UIUserNotificationAction>> y) => y.Value.Select((UIUserNotificationAction a) => a.toDict()).ToList());
		}
		return dictionary2;
	}
}
