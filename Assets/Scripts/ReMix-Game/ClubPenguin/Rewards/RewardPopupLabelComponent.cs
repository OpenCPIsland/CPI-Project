using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Rewards
{
	public class RewardPopupLabelComponent : MonoBehaviour
	{
		public Image ImageIcon;

		public Text LabelText;

		public void Init(Sprite icon, string label)
		{
			if (ImageIcon != null && icon != null)
			{
				ImageIcon.sprite = icon;
			}
			if (LabelText != null && !string.IsNullOrEmpty(label))
			{
				LabelText.text = label;
			}
		}
	}
}
