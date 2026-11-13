using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Gui
{
	public class TintToggleGroupButton_Text_TMP : MonoBehaviour
	{
		public Color OnTint;

		public Color OffTint;

		public TextMeshProUGUI text;

		public Button button;

		private void SetTint(bool on)
		{
			text.color = (on ? OnTint : OffTint);
		}

		private void Awake()
		{
			if (button == null)
			{
				button = GetComponent<Button>();
			}
			if (text == null)
			{
				text = GetComponentInChildren<TextMeshProUGUI>();
			}
			button.onClick.AddListener(OnClick);
			if (base.transform.parent != null)
			{
				TintToggleGroupButton_Text_TMP[] componentsInChildren = base.transform.parent.GetComponentsInChildren<TintToggleGroupButton_Text_TMP>(true);
				if (componentsInChildren[0] == this)
				{
					OnClick();
				}
			}
		}

		public void OnClick()
		{
			for (int i = 0; i < base.transform.parent.GetComponentsInChildren<TintToggleGroupButton_Text_TMP>(true).Length; i++)
			{
				base.transform.parent.GetComponentsInChildren<TintToggleGroupButton_Text_TMP>(true)[i].SetTint(false);
			}
			SetTint(true);
		}

		private void OnDestroy()
		{
			button.onClick.RemoveListener(OnClick);
		}
	}
}
