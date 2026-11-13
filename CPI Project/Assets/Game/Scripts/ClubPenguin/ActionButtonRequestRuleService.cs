using ClubPenguin.Core;
using System.Collections.Generic;

namespace ClubPenguin
{
	public class ActionButtonRequestRuleService
	{
		private Dictionary<string, ActionButtonRequestRuleDefinition> ruleDictionary = new Dictionary<string, ActionButtonRequestRuleDefinition>();

		public ActionButtonRequestRuleService(Manifest manifest)
		{
			for (int i = 0; i < manifest.Assets.Length; i++)
			{
				ActionButtonRequestRuleDefinition actionButtonRequestRuleDefinition = manifest.Assets[i] as ActionButtonRequestRuleDefinition;
				if (actionButtonRequestRuleDefinition != null)
				{
					ruleDictionary[actionButtonRequestRuleDefinition.Category] = actionButtonRequestRuleDefinition;
				}
			}
		}

		public bool ContainsRule(string identifier)
		{
			return ruleDictionary.ContainsKey(identifier);
		}

		public ActionButtonRequestRuleDefinition GetRule(string identifier)
		{
			return ruleDictionary[identifier];
		}
	}
}
