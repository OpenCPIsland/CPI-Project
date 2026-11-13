using Disney.Kelowna.Common.DataModel;
using System;
using UnityEngine;

namespace ClubPenguin
{
	[Serializable]
	public class PlayerStatusData : ScopedData
	{
		[SerializeField]
		private string questMascotName;

		public string QuestMascotName
		{
			get
			{
				return questMascotName;
			}
			set
			{
				if (questMascotName != value)
				{
					if (this.OnQuestMascotNameChanged != null)
					{
						this.OnQuestMascotNameChanged(this, value);
					}
					questMascotName = value;
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
				return typeof(PlayerStatusDataMonoBehaviour);
			}
		}

		public event Action<PlayerStatusData, string> OnQuestMascotNameChanged;

		protected override void notifyWillBeDestroyed()
		{
			this.OnQuestMascotNameChanged = null;
		}
	}
}
