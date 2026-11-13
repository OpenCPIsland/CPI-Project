using UnityEngine;
using TMPro;

namespace Tweaker.UI
{
	public class TileUIView : MonoBehaviour
	{
		public TextMeshProUGUI NameText;

		public string Name
		{
			get
			{
				return NameText.text;
			}
			set
			{
				NameText.text = value;
			}
		}
	}
}
