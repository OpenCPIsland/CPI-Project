namespace ClubPenguin.Collectibles
{
	public struct RespawnResponse
	{
		public readonly RespawnState State;

		public readonly long Time;

		public RespawnResponse(RespawnState state, long time = 0L)
		{
			State = state;
			Time = time;
		}
	}
}
