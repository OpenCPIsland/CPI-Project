using System.Collections;

namespace Disney.LaunchPadFramework
{
	public interface IAction
	{
		string ActionName
		{
			get;
		}

		ActionState State
		{
			get;
			set;
		}

		EventDispatcher EventDispatcher
		{
			get;
			set;
		}

		void Begin();

		bool CanBegin();

		IEnumerator Perform();

		void OnComplete();

		IEnumerator WaitTillComplete();
	}
}
