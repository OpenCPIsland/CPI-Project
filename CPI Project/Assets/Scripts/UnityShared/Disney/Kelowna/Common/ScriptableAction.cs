using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace Disney.Kelowna.Common
{
	public abstract class ScriptableAction : ScriptableObject
	{
		public abstract IEnumerator Execute(ScriptableActionPlayer player);

		[Conditional("DO_LOGGING")]
		protected void Trace(string message)
		{
		}
	}
}
