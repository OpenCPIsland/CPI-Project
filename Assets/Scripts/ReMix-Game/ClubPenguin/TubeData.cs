using Disney.Kelowna.Common.DataModel;
using System;
using UnityEngine;

namespace ClubPenguin
{
	[Serializable]
	public class TubeData : BaseData
	{
		[SerializeField]
		private int selectedTubeId;

		public int SelectedTubeId
		{
			get
			{
				return selectedTubeId;
			}
			set
			{
				if (selectedTubeId != value)
				{
					selectedTubeId = value;
					if (this.OnTubeSelected != null)
					{
						this.OnTubeSelected(selectedTubeId);
					}
				}
			}
		}

		protected override Type monoBehaviourType
		{
			get
			{
				return typeof(TubeDataMonoBehaviour);
			}
		}

		public event Action<int> OnTubeSelected;

		protected override void notifyWillBeDestroyed()
		{
			this.OnTubeSelected = null;
		}
	}
}
