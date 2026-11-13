namespace UnityEngine.UI.Extensions
{
	[RequireComponent(typeof(Text))]
	[AddComponentMenu("UI/Extensions/PPIViewer")]
	public class PPIViewer : MonoBehaviour
	{
		private Text label;

		private void Awake()
		{
			label = GetComponent<Text>();
		}

		private void Start()
		{
			if (label != null)
			{
				label.text = "PPI: " + Screen.dpi;
			}
		}
	}
}
