using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(UIWidget))]
[AddComponentMenu("NGUI/Examples/Set Color on Selection")]
public class SetColorOnSelection : MonoBehaviour
{
	private UIWidget mWidget;

	public void SetSpriteBySelection()
	{
		if (!(UIPopupList.current == null))
		{
			if (mWidget == null)
			{
				mWidget = GetComponent<UIWidget>();
			}
			switch (UIPopupList.current.value)
			{
			case "White":
				mWidget.color = Color.white;
				break;
			case "Red":
				mWidget.color = Color.red;
				break;
			case "Green":
				mWidget.color = Color.green;
				break;
			case "Blue":
				mWidget.color = Color.blue;
				break;
			case "Yellow":
				mWidget.color = Color.yellow;
				break;
			case "Cyan":
				mWidget.color = Color.cyan;
				break;
			case "Magenta":
				mWidget.color = Color.magenta;
				break;
			}
		}
	}
}
