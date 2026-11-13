namespace ClubPenguin.UI
{
	public class DefaultValuesPooledLayoutElement : AbstractPooledLayoutElement
	{
		public float DefaultWidth;

		public float DefaultHeight;

		public override float GetPreferredWidth(int elementCount, bool ignoreRestrictions = false)
		{
			return DefaultWidth;
		}

		public override float GetPreferredHeight(int elementCount)
		{
			return DefaultHeight;
		}
	}
}
