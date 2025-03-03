namespace ClubPenguin.Input
{
	public abstract class Input<TResult> : InputLib where TResult : InputResult<TResult>, new()
	{
		protected TResult inputEvent;

		private bool hasInput;

		private bool processed;

		public override void Initialize(KeyCodeRemapper keyCodeRemapper)
		{
			inputEvent = new TResult();
		}

		public override void StartFrame()
		{
			processed = false;
			hasInput = false;
		}

		public override void EndFrame()
		{
			if (!processed)
			{
				resetInput();
			}
		}

		public bool ProcessInput(TResult copyToInputEvent, int filter = -1)
		{
			if (!processed)
			{
				hasInput = process(filter);
				processed = true;
			}
			if (hasInput)
			{
				inputEvent.CopyTo(copyToInputEvent);
			}
			else
			{
				copyToInputEvent.Reset();
			}
			return hasInput;
		}

		protected virtual void resetInput()
		{
			inputEvent.Reset();
		}

		protected abstract bool process(int filter);
	}
}
