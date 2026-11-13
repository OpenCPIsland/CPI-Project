using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Gui
{
	public class TintToggleGroupButton : MonoBehaviour
	{
		public Color OnTint;

		public Color OffTint;

		public Image image;

		public Button button;

		[Header("Additional Tinting")]
		[SerializeField]
		private bool affectText = false;

		[SerializeField]
		private Text text;

		[SerializeField]
		private Color textOnTint;

		[SerializeField]
		private Color textOffTint;

		public void SetTint(bool on)
		{
			if (image != null)
			{
				image.color = (on ? OnTint : OffTint);
			}
			if (affectText && text != null)
			{
				text.color = (on ? textOnTint : textOffTint);
			}
		}

		private void Awake()
		{
			if (button == null)
			{
				button = GetComponent<Button>();
			}
			if (image == null)
			{
				image = GetComponent<Image>();
			}
			button.onClick.AddListener(OnClick);
			if (base.transform.parent != null)
			{
				TintToggleGroupButton[] componentsInChildren = base.transform.parent.GetComponentsInChildren<TintToggleGroupButton>(true);
				if (componentsInChildren[0] == this)
				{
					OnClick();
				}
			}
		}

		public void OnClick()
		{
			TintToggleGroupButton[] componentsInChildren = base.transform.parent.GetComponentsInChildren<TintToggleGroupButton>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].SetTint(false);
			}
			SetTint(true);
		}

		private void OnDestroy()
		{
			button.onClick.RemoveListener(OnClick);
		}
	}
}
