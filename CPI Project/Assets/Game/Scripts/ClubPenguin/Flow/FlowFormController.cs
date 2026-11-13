using Disney.Kelowna.Common;
using UnityEngine;

namespace ClubPenguin.Flow
{
	[DisallowMultipleComponent]
	public class FlowFormController : MonoBehaviour
	{
		public ScriptableAction NextAction
		{
			get;
			set;
		}

		public bool IsFinished
		{
			get;
			set;
		}
	}
}
