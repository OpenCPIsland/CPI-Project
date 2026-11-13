namespace ClubPenguin
{
	public class AvatarAnimationFrame
	{
		public readonly string StateName;

		public readonly float Time;

		public readonly int Layer;

		public AvatarAnimationFrame(string stateName, float time, int layer = -1)
		{
			StateName = stateName;
			Time = time;
			Layer = layer;
		}
	}
}
