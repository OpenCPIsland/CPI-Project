namespace UnityEngine.UI.Extensions
{
	public static class ScrollRectExtensions
	{
		public static void ScrollToTop(this ScrollRect scrollRect)
		{
			scrollRect.normalizedPosition = new Vector2(0f, 1f);
		}

		public static void ScrollToBottom(this ScrollRect scrollRect)
		{
			scrollRect.normalizedPosition = new Vector2(0f, 0f);
		}
	}
}
