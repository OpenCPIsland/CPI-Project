using UnityEngine;

namespace Disney.Kelowna.Common.SEDFSM
{
	public abstract class ActiveStateHandler : MonoBehaviour
	{
		public string HandledState;

		public abstract string HandleStateChange();
	}
}
