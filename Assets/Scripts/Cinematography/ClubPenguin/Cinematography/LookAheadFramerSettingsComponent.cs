using ClubPenguin.Core;
using UnityEngine;

namespace ClubPenguin.Cinematography
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(LookAheadFramer))]
	public class LookAheadFramerSettingsComponent : AspectRatioSpecificSettingsComponent<LookAheadFramer, LookAheadFramerSettings>
	{
		protected override void applySettings(LookAheadFramer component, LookAheadFramerSettings settings)
		{
			component.Offset = settings.Offset;
			component.OffsetWithKeyboard = settings.OffsetWithKeyboard;
		}
	}
}
