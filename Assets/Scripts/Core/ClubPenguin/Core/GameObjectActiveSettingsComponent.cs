using UnityEngine;

namespace ClubPenguin.Core
{
	public class GameObjectActiveSettingsComponent : PlatformSpecificSettingsComponent<Transform, GameObjectActiveSettings>
	{
		protected override void applySettings(Transform component, GameObjectActiveSettings settings)
		{
			component.gameObject.SetActive(settings.IsActive);
		}
	}
}
