namespace ClubPenguin.DCE
{
	public class DceAnimationFrame
	{
		public readonly string StateName;

		public readonly float Time;

		public readonly int Layer;

		public DceAnimationFrame(string stateName, float time, int layer = -1)
		{
			StateName = stateName;
			Time = time;
			Layer = layer;
		}
	}
}
