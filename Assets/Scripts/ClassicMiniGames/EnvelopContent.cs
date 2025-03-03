using UnityEngine;

[RequireComponent(typeof(UIWidget))]
[AddComponentMenu("NGUI/Examples/Envelop Content")]
public class EnvelopContent : MonoBehaviour
{
	public Transform targetRoot;

	public int padLeft = 0;

	public int padRight = 0;

	public int padBottom = 0;

	public int padTop = 0;

	private bool mStarted = false;

	private void Start()
	{
		mStarted = true;
		Execute();
	}

	private void OnEnable()
	{
		if (mStarted)
		{
			Execute();
		}
	}

	[ContextMenu("Execute")]
	public void Execute()
	{
		if (targetRoot == base.transform)
		{
			Debug.LogError("Target Root object cannot be the same object that has Envelop Content. Make it a sibling instead.", this);
			return;
		}
		if (NGUITools.IsChild(targetRoot, base.transform))
		{
			Debug.LogError("Target Root object should not be a parent of Envelop Content. Make it a sibling instead.", this);
			return;
		}
		Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(base.transform.parent, targetRoot, false);
		float num = bounds.min.x + (float)padLeft;
		float num2 = bounds.min.y + (float)padBottom;
		float num3 = bounds.max.x + (float)padRight;
		float num4 = bounds.max.y + (float)padTop;
		UIWidget component = GetComponent<UIWidget>();
		component.SetRect(num, num2, num3 - num, num4 - num2);
		BroadcastMessage("UpdateAnchors", SendMessageOptions.DontRequireReceiver);
	}
}
