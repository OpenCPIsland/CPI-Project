using System.Collections;

namespace DisneyMobile.CoreUnitySystems
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

		void Start();

		bool CanStart();

		IEnumerator Perform();

		void OnComplete();

		IEnumerator WaitTillComplete();
	}
}
