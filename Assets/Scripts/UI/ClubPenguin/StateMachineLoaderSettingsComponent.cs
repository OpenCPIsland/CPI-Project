using ClubPenguin.Core;
using Disney.Kelowna.Common.SEDFSM;
using UnityEngine;

namespace ClubPenguin
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(StateMachineLoader))]
	public class StateMachineLoaderSettingsComponent : AspectRatioSpecificSettingsComponent<StateMachineLoader, StateMachineLoaderSettings>
	{
		protected override void applySettings(StateMachineLoader component, StateMachineLoaderSettings settings)
		{
			for (int i = 0; i < settings.BindingOverrides.Length; i++)
			{
				StateMachineLoader.Binding binding = settings.BindingOverrides[i];
				for (int j = 0; j < component.Bindings.Length; j++)
				{
					if (binding.Name == component.Bindings[j].Name)
					{
						component.Bindings[j] = binding;
					}
				}
			}
		}
	}
}
