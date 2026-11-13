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

        private void HideCurrentTooltip()
        {
            if (currentTooltip == null)
            {
                return;
            }

            TutorialTooltip tooltip = currentTooltip.GetComponent<TutorialTooltip>();

            if (tooltip != null)
            {
                tooltip.Hide();
                return;
            }

            TutorialTooltipTMP tmpTooltip = currentTooltip.GetComponent<TutorialTooltipTMP>();

            if (tmpTooltip != null)
            {
                tmpTooltip.Hide();
            }
        }

        private void SetupTooltip(GameObject tooltipObject, Vector2 position)
        {
            TutorialTooltip tooltip = tooltipObject.GetComponent<TutorialTooltip>();

            if (tooltip != null)
            {
                tooltip.transform.SetParent(base.transform, false);
                tooltip.SetPosition(position);
                tooltip.SetDefaultTextPrefab(defaultTextPrefab);
                tooltip.Show();
                return;
            }

            TutorialTooltipTMP tmpTooltip = tooltipObject.GetComponent<TutorialTooltipTMP>();

            if (tmpTooltip != null)
            {
                tmpTooltip.transform.SetParent(base.transform, false);
                tmpTooltip.SetPosition(position);
                tmpTooltip.SetDefaultTextPrefab(defaultTextPrefab);
                tmpTooltip.Show();
            }
        }

        private GameObject showTooltip(TutorialTooltip tooltip, RectTransform target, Vector2 offset, bool fullScreenClose)
        {
            HideCurrentTooltip();

            Vector2 v = Vector2.zero;

            if (target != null)
            {
                v = new Vector2(target.position.x, target.position.y);

                Canvas canvas = target.GetComponentInParent<Canvas>();

                if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceCamera)
                {
                    v = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, v);
                }
            }

            CanvasScalerExt component = GetComponentInParent<Canvas>().GetComponent<CanvasScalerExt>();

            Vector2 vector = new Vector2(
                component.ReferenceResolutionY / (float)Screen.height,
                component.ReferenceResolutionY / (float)Screen.height);

            vector *= 1f / component.ScaleModifier;

            v = new Vector2(
                (v.x + offset.x) * vector.x,
                (v.y + offset.y) * vector.y);

            GameObject gameObject;

            if (tooltip == null)
            {
                gameObject = Object.Instantiate(tooltipPrefab);
            }
            else
            {
                gameObject = tooltip.gameObject;
            }

            SetupTooltip(gameObject, v);

            currentTooltip = gameObject;

            FullScreenButton.SetActive(fullScreenClose);

            TutorialTooltip createdTooltip = gameObject.GetComponent<TutorialTooltip>();

            if (createdTooltip != null)
            {
                Service.Get<EventDispatcher>().DispatchEvent(
                    new TutorialUIEvents.OnTooltipCreated(createdTooltip));
            }

            return gameObject;
        }

        private void hideTooltip()
        {
            HideCurrentTooltip();
            FullScreenButton.SetActive(false);
        }

        private bool onShowTooltip(TutorialUIEvents.ShowTooltip evt)
        {
            TutorialTooltip tooltip = evt.Tooltip.GetComponent<TutorialTooltip>();

            if (tooltip == null)
            {
                TutorialTooltipTMP tmpTooltip = evt.Tooltip.GetComponent<TutorialTooltipTMP>();

                if (tmpTooltip != null)
                {
                    showTooltip(null, evt.Target, evt.Offset, evt.FullScreenClose);
                    return false;
                }
            }

            showTooltip(
                tooltip,
                evt.Target,
                evt.Offset,
                evt.FullScreenClose);

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