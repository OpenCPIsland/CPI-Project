namespace UnityEngine.UI.Extensions
{
	[RequireComponent(typeof(ScrollRect))]
	[AddComponentMenu("UI/Extensions/ScrollRectLinker")]
	public class ScrollRectLinker : MonoBehaviour
	{
		public bool clamp = true;

		[SerializeField]
		private ScrollRect controllingScrollRect;

		private ScrollRect scrollRect;

		private void Awake()
		{
			scrollRect = GetComponent<ScrollRect>();
			if (controllingScrollRect != null)
			{
				controllingScrollRect.onValueChanged.AddListener(MirrorPos);
			}
		}

		private void MirrorPos(Vector2 scrollPos)
		{
			if (clamp)
			{
				scrollRect.normalizedPosition = new Vector2(Mathf.Clamp01(scrollPos.x), Mathf.Clamp01(scrollPos.y));
			}
			else
			{
				scrollRect.normalizedPosition = scrollPos;
			}
		}
	}
}
