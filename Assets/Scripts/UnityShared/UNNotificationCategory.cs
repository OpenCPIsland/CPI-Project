using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class UNNotificationCategory
{
	private const string IDENTIFIER_KEY = "identifier";

	private const string OPTIONS_KEY = "options";

	private const string ACTIONS_KEY = "actions";

	public string identifier;

	public List<UNNotificationCategoryOptions> options;

	public List<UNNotificationAction> actions;

	public Dictionary<string, object> toDict()
	{
		List<string> list = new List<string>();
		if (0 < options.Count)
		{
			for (int i = 0; i < options.Count; i++)
			{
				list.Add(ExtractOptionFromEnum(options[i]));
			}
		}
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary["identifier"] = identifier;
		dictionary["options"] = list;
		dictionary["actions"] = actions.Select((UNNotificationAction a) => a.toDict()).ToList();
		return dictionary;
	}

	private string ExtractOptionFromEnum(UNNotificationCategoryOptions option)
	{
		switch (option)
		{
		case UNNotificationCategoryOptions.UNNotificationCategoryOptionCustomDismissAction:
			return "custom_dismiss";
		case UNNotificationCategoryOptions.UNNotificationCategoryOptionAllowInCarPlay:
			return "carplay";
		default:
			return "";
		}
	}
}
