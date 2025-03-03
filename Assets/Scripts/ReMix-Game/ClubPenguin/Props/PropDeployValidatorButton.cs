using ClubPenguin.UI;
using Disney.Kelowna.Common;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Props
{
	[RequireComponent(typeof(InputButtonMapper))]
	public class PropDeployValidatorButton : MonoBehaviour
	{
		public PrefabContentKey TooltipPrefab;

		public Vector2 TooltipOffset;

		private Button button;

		private InputButton inputButton;

		private TrayInputButton trayInputButton;

		private InteractButtonToggle interactButtonToggle;

		private TooltipInputButton tooltipButton;

		private GameObject tooltip;

		private PropValidateDeployLocation propValidator;

		private PropSpawnLocationValidator validator;

		private void Start()
		{
			button = GetComponentInParent<Button>();
			inputButton = button.GetComponent<InputButton>();
			trayInputButton = button.GetComponent<TrayInputButton>();
			interactButtonToggle = GetComponent<InteractButtonToggle>();
			tooltipButton = button.gameObject.AddComponent<TooltipInputButton>();
			PropUser component = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject.GetComponent<PropUser>();
			Prop prop = component.Prop;
			if (prop != null)
			{
				propValidator = prop.GetComponent<PropValidateDeployLocation>();
				if (propValidator != null)
				{
					validator = propValidator.GetValidator();
					if (validator == null)
					{
						propValidator.OnValidatorSpawned += onValidatorSpawned;
					}
					else
					{
						onValidPositionChanged(validator.IsValidPosition);
						validator.OnValidPositionChanged += onValidPositionChanged;
					}
				}
			}
			Content.LoadAsync(onTooltipLoaded, TooltipPrefab);
		}

		private void onTooltipLoaded(string path, GameObject prefab)
		{
			tooltip = Object.Instantiate(prefab, button.transform.parent.parent, false);
			tooltip.transform.localPosition = new Vector3(TooltipOffset.x, TooltipOffset.y, 0f);
			tooltipButton.TooltipAnimator = tooltip.GetComponent<Animator>();
		}

		private void OnDestroy()
		{
			if (validator != null)
			{
				validator.OnValidPositionChanged -= onValidPositionChanged;
			}
		}

		private void onValidatorSpawned(PropSpawnLocationValidator validator)
		{
			if (propValidator != null)
			{
				propValidator.OnValidatorSpawned -= onValidatorSpawned;
			}
			this.validator = validator;
			this.validator.OnValidPositionChanged += onValidPositionChanged;
			onValidPositionChanged(validator.IsValidPosition);
		}

		private void onValidPositionChanged(bool isValidPosition)
		{
			if (isValidPosition)
			{
				trayInputButton.SetState(TrayInputButton.ButtonState.Pulsing);
				inputButton.enabled = true;
				tooltipButton.TooltipEnabled = false;
				interactButtonToggle.enabled = true;
			}
			else
			{
				trayInputButton.SetState(TrayInputButton.ButtonState.Default);
				inputButton.enabled = false;
				tooltipButton.TooltipEnabled = true;
				interactButtonToggle.enabled = false;
			}
		}
	}
}
