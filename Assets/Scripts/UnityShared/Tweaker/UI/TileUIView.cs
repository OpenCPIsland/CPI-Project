using UnityEngine;
using UnityEngine.UI;

namespace Tweaker.UI
{
	public class TileUIView : MonoBehaviour
	{
		public Text NameText;

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
