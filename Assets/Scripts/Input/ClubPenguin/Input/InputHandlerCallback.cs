using System;

namespace ClubPenguin.Input
{
	public struct InputHandlerCallback<TResult>
	{
		public readonly Action<TResult> OnHandle;

		public readonly Action OnReset;

		public InputHandlerCallback(Action<TResult> onHandle, Action onReset)
		{
			OnHandle = onHandle;
			OnReset = onReset;
		}
	}
}
