using Disney.Kelowna.Common.DataModel;
using System;
using UnityEngine;

namespace ClubPenguin
{
	[Serializable]
	public class DisplayNameData : BaseData, IEntityIdentifierData<string>
	{
		[SerializeField]
		private string displayName;

		public string DisplayName
		{
			get
			{
				return displayName;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					if (value != displayName)
					{
						if (this.OnDisplayNameChanged != null)
						{
							this.OnDisplayNameChanged(value);
						}
						displayName = value;
					}
					return;
				}
				throw new Exception("Display name cannot be null or empty");
			}
		}

		protected override Type monoBehaviourType
		{
			get
			{
				return typeof(DisplayNameMonoBehaviour);
			}
		}

		public event Action<string> OnDisplayNameChanged;

		public bool Match(string identifier)
		{
			return identifier == displayName;
		}

		protected override void notifyWillBeDestroyed()
		{
			this.OnDisplayNameChanged = null;
		}
	}
}
