namespace ClubPenguin.WorldEditor.Optimization
{
	public class RemoveOccludedGPU : RemoveOccluded
	{
		public int MaxTrianglesPerDispatch = 10000;

		public override void Begin(GameObjectData[] datas)
		{
		}

		public override void Execute(Visibility visibility)
		{
		}

		public override void End()
		{
		}
	}
}
