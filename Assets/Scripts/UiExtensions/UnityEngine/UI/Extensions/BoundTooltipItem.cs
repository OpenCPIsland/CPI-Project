namespace UnityEngine.UI.Extensions
{
	[AddComponentMenu("UI/Extensions/Bound Tooltip/Tooltip Item")]
	public class BoundTooltipItem : MonoBehaviour
	{
		public Text TooltipText;

		public Vector3 ToolTipOffset;

		private static BoundTooltipItem instance;

		public bool IsActive
		{
			get
			{
				return base.gameObject.activeSelf;
			}
		}

		public static BoundTooltipItem Instance
		{
			get
			{
				if (instance == null)
				{
					instance = Object.FindObjectOfType<BoundTooltipItem>();
				}
				return instance;
			}
		}

		private void Awake()
		{
			instance = this;
			if (!TooltipText)
			{
				TooltipText = GetComponentInChildren<Text>();
			}
			HideTooltip();
		}

		public void ShowTooltip(string text, Vector3 pos)
		{
			if (TooltipText.text != text)
			{
				TooltipText.text = text;
			}
			base.transform.position = pos + ToolTipOffset;
			base.gameObject.SetActive(true);
		}

		public void HideTooltip()
		{
			base.gameObject.SetActive(false);
		}
	}
}
