using Disney.Kelowna.Common;
using System.Collections.Generic;

namespace ClubPenguin.Input
{
	public abstract class InputMap<TResult> : InputMapLib where TResult : new()
	{
		protected readonly TResult mapResult = new TResult();

		private readonly List<InputHandlerCallback<TResult>> inputHandlers = new List<InputHandlerCallback<TResult>>();

		private bool hasHadInputHandled;

		public override bool ProcessInput(ControlScheme controlScheme)
		{
			bool flag = base.Enabled && processInput(controlScheme);
			if (flag)
			{
				handleInput();
			}
			return flag;
		}

		private void handleInput()
		{
			if (inputHandlers.Count > 0)
			{
				inputHandlers[0].OnHandle.InvokeSafe(mapResult);
				hasHadInputHandled = true;
			}
		}

		public virtual void AddHandler(InputHandlerCallback<TResult> handler)
		{
			if (!inputHandlers.Contains(handler))
			{
				ResetInput();
				inputHandlers.Insert(0, handler);
				base.Enabled |= autoEnabled;
			}
		}

		public void RemoveHandler(InputHandlerCallback<TResult> handler)
		{
			if (inputHandlers.Count > 0 && inputHandlers[0].Equals(handler))
			{
				ResetInput();
			}
			inputHandlers.Remove(handler);
			if (inputHandlers.Count == 0 && autoEnabled)
			{
				base.Enabled = false;
			}
		}

		public override void ResetInput()
		{
			if (hasHadInputHandled && inputHandlers.Count > 0)
			{
				inputHandlers[0].OnReset.InvokeSafe();
				hasHadInputHandled = false;
			}
		}

		protected abstract bool processInput(ControlScheme controlScheme);
	}
}
