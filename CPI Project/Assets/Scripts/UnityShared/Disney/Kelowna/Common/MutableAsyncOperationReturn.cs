using UnityEngine;

namespace Disney.Kelowna.Common
{
	public class MutableAsyncOperationReturn : AsyncOperationReturn
	{
		public AsyncOperation MutableOperation;

		public override bool Finished
		{
			get
			{
				if (MutableOperation == null)
				{
					return false;
				}
				return MutableOperation.isDone;
			}
		}

		public override bool Cancelled
		{
			get
			{
				if (MutableOperation == null)
				{
					return false;
				}
				return base.Cancelled;
			}
		}

		public MutableAsyncOperationReturn(AsyncOperation operation)
			: base(operation)
		{
			MutableOperation = operation;
		}
	}
}
