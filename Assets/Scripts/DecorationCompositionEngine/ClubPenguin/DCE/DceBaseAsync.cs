using System;
using UnityEngine;

namespace ClubPenguin.DCE
{
	public abstract class DceBaseAsync : MonoBehaviour
	{
		public bool IsReady
		{
			get;
			private set;
		}

		public bool IsBusy
		{
			get;
			private set;
		}

		public event Action<DceBaseAsync> OnBusy;

		public event Action<DceBaseAsync> OnReady;

		protected void startWork()
		{
			IsReady = false;
			IsBusy = true;
			if (this.OnBusy != null)
			{
				this.OnBusy(this);
			}
		}

		protected void stopWork()
		{
			IsReady = true;
			IsBusy = false;
			if (this.OnReady != null)
			{
				this.OnReady(this);
			}
		}
	}
}
