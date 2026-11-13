using System;
using UnityEngine;

namespace ClubPenguin.CellPhone
{
	public class CellPhoneIntroAnimation : MonoBehaviour
	{
		public event Action EIntroAnimCompleted;

		public event Action EOutroAnimCompleted;

		public void OnIntroAnimComplete()
		{
			if (this.EIntroAnimCompleted != null)
			{
				this.EIntroAnimCompleted();
			}
		}

		public void OnOutroAnimComplete()
		{
			if (this.EOutroAnimCompleted != null)
			{
				this.EOutroAnimCompleted();
			}
		}
	}
}
