using System;

namespace Disney.Kelowna.Common.DataModel
{
	[Serializable]
	public abstract class BaseData
	{
		protected virtual Type monoBehaviourType
		{
			get
			{
				return null;
			}
		}

		internal Type MonoBehaviourType
		{
			get
			{
				return monoBehaviourType;
			}
		}

		internal void NotifyWillBeDestroyed()
		{
			notifyWillBeDestroyed();
		}

		protected abstract void notifyWillBeDestroyed();
	}
}
