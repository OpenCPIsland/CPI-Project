namespace ClubPenguin
{
	public struct AwayFromKeyboardEvent
	{
		public readonly AwayFromKeyboardStateType Type;

		public readonly bool FaceCamera;

		public AwayFromKeyboardEvent(AwayFromKeyboardStateType type, bool faceCamera = false)
		{
			Type = type;
			FaceCamera = faceCamera;
		}
	}
}
