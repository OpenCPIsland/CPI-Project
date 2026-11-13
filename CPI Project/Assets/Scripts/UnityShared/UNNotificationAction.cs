using System;
using System.Collections.Generic;

[Serializable]
public class UNNotificationAction
{
	private const string IDENTIFIER_KEY = "identifier";

	private const string TITLE_KEY = "title";

	private const string OPTIONS_KEY = "options";

	public string identifier;

	public string title;

	public List<UNUserNotificationAction> options;

	public Dictionary<string, object> toDict()
	{
		List<string> list = new List<string>();
		if (0 < options.Count)
		{
			for (int i = 0; i < options.Count; i++)
			{
				list.Add(ExtractActionFromEnum(options[i]));
			}
		}
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary["identifier"] = identifier;
		dictionary["title"] = title;
		dictionary["options"] = list;
		return dictionary;
	}

	private string ExtractActionFromEnum(UNUserNotificationAction action)
	{
		switch (action)
		{
		case UNUserNotificationAction.UNNotificationActionOptionForeground:
			return "foreground";
		case UNUserNotificationAction.UNNotificationActionOptionDestructive:
			return "destructive";
		case UNUserNotificationAction.UNNotificationActionOptionAuthenticationRequired:
			return "auth-required";
		default:
			return "";
		}
	}
}
