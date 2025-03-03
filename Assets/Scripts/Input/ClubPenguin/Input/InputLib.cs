using UnityEngine;

namespace ClubPenguin.Input
{
	public abstract class InputLib : ScriptableObject
	{
		public abstract void Initialize(KeyCodeRemapper keyCodeRemapper);

		public abstract void StartFrame();

		public abstract void EndFrame();
	}
}
