using Disney.Kelowna.Common.DataModel;
using System;

namespace ClubPenguin.UI
{
	[Serializable]
	public class SystemBarsData : BaseData
	{
		public Action<int> OnShown;

		public Action OnHidden;

		public bool IsOpen
		{
			get;
			private set;
		}

		public int NavigationBarHeight
		{
			get;
			private set;
		}

		public int CurrentNavigationBarHeight
		{
			get
			{
				return IsOpen ? NavigationBarHeight : 0;
			}
		}

		protected override Type monoBehaviourType
		{
			get
			{
				return typeof(SystemBarsDataMonoBehaviour);
			}
		}

		public void SetIsOpen(bool isOpen, int navigationBarHeight = 0)
		{
			IsOpen = isOpen;
			NavigationBarHeight = navigationBarHeight;
			if (isOpen)
			{
				if (OnShown != null)
				{
					OnShown(navigationBarHeight);
				}
			}
			else if (OnHidden != null)
			{
				OnHidden();
			}
		}

		protected override void notifyWillBeDestroyed()
		{
			OnShown = null;
			OnHidden = null;
		}
	}
}
