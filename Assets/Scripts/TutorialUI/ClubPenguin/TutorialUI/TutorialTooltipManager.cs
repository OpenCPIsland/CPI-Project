using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.TutorialUI
{
	[RequireComponent(typeof(Canvas))]
	public class TutorialTooltipManager : MonoBehaviour
	{
		private static PrefabContentKey tooltipContentKey = new PrefabContentKey("Prefabs/Tooltip");

		private static PrefabContentKey tooltipDefaultTextContentKey = new PrefabContentKey("Prefabs/TooltipDefaultText");

		public GameObject FullScreenButton;

		private GameObject tooltipPrefab;

		private GameObject defaultTextPrefab;

		private GameObject currentTooltip;

		private EventChannel eventChannel;

		private void Start()
		{
			eventChannel = new EventChannel(Service.Get<EventDispatcher>());
			eventChannel.AddListener<TutorialUIEvents.ShowTooltip>(onShowTooltip);
			eventChannel.AddListener<TutorialUIEvents.HideTooltip>(onHideTooltip);
			Content.LoadAsync(onPrefabLoaded, tooltipContentKey);
			Content.LoadAsync(onTextPrefabLoaded, tooltipDefaultTextContentKey);
			FullScreenButton.SetActive(false);
		}

		private void OnDestroy()
		{
			eventChannel.RemoveAllListeners();
		}

		private void onPrefabLoaded(string path, GameObject prefab)
		{
			tooltipPrefab = prefab;
		}

		private void onTextPrefabLoaded(string path, GameObject prefab)
		{
			defaultTextPrefab = prefab;
		}

		private GameObject showTooltip(TutorialTooltip tooltip, RectTransform target, Vector2 offset, bool fullScreenClose)
		{
			if (currentTooltip != null)
			{
				currentTooltip.GetComponent<TutorialTooltip>().Hide();
			}
			Vector2 v = Vector2.zero;
			if (target != null)
			{
				v = new Vector2(target.position.x, target.position.y);
				Canvas componentInParent = target.GetComponentInParent<Canvas>();
				if (componentInParent.renderMode == RenderMode.ScreenSpaceCamera)
				{
					v = RectTransformUtility.WorldToScreenPoint(componentInParent.worldCamera, v);
				}
			}
			CanvasScalerExt component = GetComponentInParent<Canvas>().GetComponent<CanvasScalerExt>();
			Vector2 vector = new Vector2(component.ReferenceResolutionY / (float)Screen.height, component.ReferenceResolutionY / (float)Screen.height);
			vector *= 1f / component.ScaleModifier;
			v = new Vector2((v.x + offset.x) * vector.x, (v.y + offset.y) * vector.y);
			if (tooltip == null)
			{
				GameObject gameObject = Object.Instantiate(tooltipPrefab);
				tooltip = gameObject.GetComponent<TutorialTooltip>();
			}
			tooltip.transform.SetParent(base.transform, false);
			tooltip.SetPosition(v);
			tooltip.SetDefaultTextPrefab(defaultTextPrefab);
			tooltip.Show();
			currentTooltip = tooltip.gameObject;
			FullScreenButton.SetActive(fullScreenClose);
			Service.Get<EventDispatcher>().DispatchEvent(new TutorialUIEvents.OnTooltipCreated(tooltip));
			return tooltip.gameObject;
		}

		private void hideTooltip()
		{
			if (currentTooltip != null)
			{
				currentTooltip.GetComponent<TutorialTooltip>().Hide();
			}
			FullScreenButton.SetActive(false);
		}

		private bool onShowTooltip(TutorialUIEvents.ShowTooltip evt)
		{
			TutorialTooltip component = evt.Tooltip.GetComponent<TutorialTooltip>();
			showTooltip(component, evt.Target, evt.Offset, evt.FullScreenClose);
			return false;
		}

		private bool onHideTooltip(TutorialUIEvents.HideTooltip evt)
		{
			hideTooltip();
			return false;
		}

		public void OnFullScreenButtonPressed()
		{
			hideTooltip();
		}
	}
}
