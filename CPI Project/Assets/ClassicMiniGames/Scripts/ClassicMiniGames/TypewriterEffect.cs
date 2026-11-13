using UnityEngine;

[RequireComponent(typeof(UILabel))]
[AddComponentMenu("NGUI/Examples/Typewriter Effect")]
public class TypewriterEffect : MonoBehaviour
{
	public int charsPerSecond = 20;

	public float delayOnPeriod = 0f;

	public float delayOnNewLine = 0f;

	public UIScrollView scrollView;

	private UILabel mLabel;

	private string mText;

	private int mOffset = 0;

	private float mNextChar = 0f;

	private bool mReset = true;

	private void OnEnable()
	{
		mReset = true;
	}

	private void Update()
	{
		if (mReset)
		{
			mOffset = 0;
			mReset = false;
			mLabel = GetComponent<UILabel>();
			mText = mLabel.processedText;
		}
		if (mOffset >= mText.Length || !(mNextChar <= RealTime.time))
		{
			return;
		}
		charsPerSecond = Mathf.Max(1, charsPerSecond);
		float num = 1f / (float)charsPerSecond;
		char c = mText[mOffset];
		if (c == '.')
		{
			if (mOffset + 2 < mText.Length && mText[mOffset + 1] == '.' && mText[mOffset + 2] == '.')
			{
				num += delayOnPeriod * 3f;
				mOffset += 2;
			}
			else
			{
				num += delayOnPeriod;
			}
		}
		else if (c == '!' || c == '?')
		{
			num += delayOnPeriod;
		}
		else if (c == '\n')
		{
			num += delayOnNewLine;
		}
		NGUIText.ParseSymbol(mText, ref mOffset);
		mNextChar = RealTime.time + num;
		mLabel.text = mText.Substring(0, ++mOffset);
		if (scrollView != null)
		{
			scrollView.UpdatePosition();
		}
	}
}
