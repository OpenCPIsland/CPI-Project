using ClubPenguin.Net.Domain.Igloo;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Igloo
{
	[Serializable]
	public class SavedIgloosMetaData : ScopedData
	{
		public bool IsDirty;

		[SerializeField]
		private long? activeIglooId;

		[SerializeField]
		private IglooVisibility iglooVisibility;

		[SerializeField]
		private List<SavedIglooMetaData> savedIgloos;

		public long? ActiveIglooId
		{
			get
			{
				return activeIglooId;
			}
			set
			{
				if (activeIglooId != value)
				{
					IsDirty = true;
				}
				activeIglooId = value;
				if (value.HasValue)
				{
					this.ActiveIglooIdUpdated.InvokeSafe(activeIglooId.Value);
				}
			}
		}

		public IglooVisibility IglooVisibility
		{
			get
			{
				return iglooVisibility;
			}
			set
			{
				if (iglooVisibility != value)
				{
					IsDirty = true;
				}
				iglooVisibility = value;
				this.PublishedStatusUpdated.InvokeSafe(value);
			}
		}

		public List<SavedIglooMetaData> SavedIgloos
		{
			get
			{
				return savedIgloos ?? new List<SavedIglooMetaData>();
			}
			set
			{
				savedIgloos = value;
				this.SavedIgloosUpdated.InvokeSafe(value);
			}
		}

		protected override string scopeID
		{
			get
			{
				return CPDataScopes.Session.ToString();
			}
		}

		protected override Type monoBehaviourType
		{
			get
			{
				return typeof(SavedIgloosMetaDataMonoBehaviour);
			}
		}

		public event Action<List<SavedIglooMetaData>> SavedIgloosUpdated;

		public event Action<IglooVisibility> PublishedStatusUpdated;

		public event Action<long> ActiveIglooIdUpdated;

		public bool IsFirstIglooLoad()
		{
			return !ActiveIglooId.HasValue || ActiveIglooId == 0;
		}

		protected override void notifyWillBeDestroyed()
		{
		}
	}
}
