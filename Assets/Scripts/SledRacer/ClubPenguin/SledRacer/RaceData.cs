using Disney.Kelowna.Common.DataModel;
using System;

namespace ClubPenguin.SledRacer
{
	[Serializable]
	public class RaceData : ScopedData
	{
		private RaceResults raceResults;

		public RaceResults RaceResults
		{
			get
			{
				return raceResults;
			}
			set
			{
				if (value != raceResults)
				{
					valueChanged(value);
					raceResults = value;
				}
			}
		}

		protected override string scopeID
		{
			get
			{
				return CPDataScopes.Zone.ToString();
			}
		}

		protected override Type monoBehaviourType
		{
			get
			{
				return typeof(RaceDataMonoBehaviour);
			}
		}

		public event Action<RaceResults> RaceResultsChanged;

		protected override void notifyWillBeDestroyed()
		{
			this.RaceResultsChanged = null;
		}

		private void valueChanged(RaceResults value)
		{
			if (this.RaceResultsChanged != null)
			{
				this.RaceResultsChanged(value);
			}
		}
	}
}
