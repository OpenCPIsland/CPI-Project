using ClubPenguin.UI;
using UnityEngine;

namespace ClubPenguin.Props
{
	public class PropControlsOverride : MonoBehaviour
	{
		[SerializeField]
		private InputButtonGroupContentKey SwimControlsScreenDefinitionContentKey;

		[SerializeField]
		private InputButtonGroupContentKey DivingControlsScreenDefinitionContentKey;

		[SerializeField]
		private InputButtonGroupContentKey ControlsScreenDefinitionContentKey;

		[SerializeField]
		private InputButtonGroupContentKey TubeControlsScreenDefinitionContentKey;

		[SerializeField]
		private InputButtonGroupContentKey SitControlsScreenDefinitionContentKey;

		[SerializeField]
		private InputButtonGroupContentKey SitSwimControlsScreenDefinitionContentKey;

		public InputButtonGroupContentKey SwimControls
		{
			get
			{
				return SwimControlsScreenDefinitionContentKey;
			}
		}

		public InputButtonGroupContentKey DivingControls
		{
			get
			{
				return DivingControlsScreenDefinitionContentKey;
			}
		}

		public InputButtonGroupContentKey DefaultControls
		{
			get
			{
				return ControlsScreenDefinitionContentKey;
			}
		}

		public InputButtonGroupContentKey TubeControls
		{
			get
			{
				return TubeControlsScreenDefinitionContentKey;
			}
		}

		public InputButtonGroupContentKey SitControls
		{
			get
			{
				return SitControlsScreenDefinitionContentKey;
			}
		}

		public InputButtonGroupContentKey SitSwimControls
		{
			get
			{
				return SitSwimControlsScreenDefinitionContentKey;
			}
		}
	}
}
