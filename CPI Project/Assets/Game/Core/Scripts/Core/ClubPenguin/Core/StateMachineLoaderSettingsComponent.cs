using Disney.Kelowna.Common.SEDFSM;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Core
{
	[RequireComponent(typeof(StateMachineLoader))]
	[DisallowMultipleComponent]
	public class StateMachineLoaderSettingsComponent : PlatformSpecificSettingsComponent<StateMachineLoader, StateMachineLoaderSettings>
	{
		protected override void applySettings(StateMachineLoader component, StateMachineLoaderSettings settings)
		{
			if (settings.Bindings == null)
			{
				return;
			}
			Dictionary<string, StateMachineDefinition> dictionary = new Dictionary<string, StateMachineDefinition>();
			for (int i = 0; i < settings.Bindings.Length; i++)
			{
				dictionary.Add(settings.Bindings[i].Name, settings.Bindings[i].Definition);
			}
			for (int i = 0; i < component.Bindings.Length; i++)
			{
				string name = component.Bindings[i].Name;
				if (dictionary.ContainsKey(name))
				{
					component.Bindings[i].Definition = dictionary[name];
					dictionary.Remove(name);
				}
			}
			if (dictionary.Count > 0)
			{
				List<StateMachineLoader.Binding> list = new List<StateMachineLoader.Binding>(component.Bindings);
				foreach (KeyValuePair<string, StateMachineDefinition> item2 in dictionary)
				{
					StateMachineLoader.Binding item = default(StateMachineLoader.Binding);
					item.Name = item2.Key;
					item.Definition = item2.Value;
					list.Add(item);
				}
				component.Bindings = list.ToArray();
			}
		}
	}
}
