public class HyperlinkButtonInfo
{
	public string Name
	{
		get;
		private set;
	}

	public float X
	{
		get;
		private set;
	}

	public float Y
	{
		get;
		private set;
	}

	public float W
	{
		get;
		private set;
	}

	public float H
	{
		get;
		private set;
	}

	public HyperlinkButtonInfo(string aName, float aX, float aY, float aW, float aH)
	{
		Name = aName;
		X = aX;
		Y = aY;
		W = aW;
		H = aH;
	}
}
