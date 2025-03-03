using UnityEngine;

namespace Disney.Kelowna.Common.SEDFSM
{
	public abstract class PassiveStateHandler : MonoBehaviour
	{
		public abstract void HandleStateChange(string newState);
	}
}
