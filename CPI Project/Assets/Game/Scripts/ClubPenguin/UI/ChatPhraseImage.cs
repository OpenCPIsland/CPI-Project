using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
    public class ChatPhraseImage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler, IEventSystemHandler
    {
        [SerializeField]
        private GameObject content;

        [SerializeField]
        private float expandSize;

        private RectTransform rectTransform;

        private float originalWidth;

        private float originalHeight;

        private void Awake()
        {
            base.gameObject.SetActive(false);
        }

        public void UpdatePosition()
        {
            if (this == null)
            {
                return;
            }

            base.gameObject.SetActive(true);

            VerticalLayoutGroup componentInParent = GetComponentInParent<VerticalLayoutGroup>();
            this.rectTransform = base.transform as RectTransform;

            if (content == null || componentInParent == null || this.rectTransform == null)
            {
                return;
            }

            RectTransform rectTransform = content.transform as RectTransform;

            if (rectTransform == null)
            {
                return;
            }

            originalWidth = rectTransform.rect.width + componentInParent.padding.left + componentInParent.padding.right;
            originalHeight = this.rectTransform.rect.height;

            scaleNormal();
            updatePosition(originalWidth, originalHeight);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (this == null)
            {
                return;
            }

            scaleUp();
        }

        public void OnSelect(BaseEventData eventData)
        {
            if (this == null)
            {
                return;
            }

            scaleUp();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (this == null)
            {
                return;
            }

            scaleNormal();
        }

        public void OnDeselect(BaseEventData eventData)
        {
            if (this == null)
            {
                return;
            }

            scaleNormal();
        }

        private void scaleUp()
        {
            if (this == null || rectTransform == null)
            {
                return;
            }

            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, originalWidth + expandSize);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, originalHeight + expandSize);
        }

        private void scaleNormal()
        {
            if (this == null || rectTransform == null)
            {
                return;
            }

            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, originalWidth);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, originalHeight);
        }

        private void updatePosition(float width, float height)
        {
            if (this == null || rectTransform == null)
            {
                return;
            }

            rectTransform.anchoredPosition = new Vector3(width * 0.5f, (0f - height) * 0.5f, rectTransform.localPosition.z);
        }
    }
}