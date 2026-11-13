using UnityEngine;

namespace Disney.Kelowna.Common
{
	public class AsyncOperationReturn : CoroutineReturn
	{
		public readonly AsyncOperation Operation;

		public override bool Finished
		{
			get
			{
				return Operation.isDone;
			}
		}

		public AsyncOperationReturn(AsyncOperation operation)
		{
			Operation = operation;
		}
	}
}
