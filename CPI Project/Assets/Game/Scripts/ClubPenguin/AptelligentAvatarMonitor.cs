using ClubPenguin.Avatar;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin
{
	[RequireComponent(typeof(AvatarModel))]
	public class AptelligentAvatarMonitor : MonoBehaviour
	{
		public AvatarModel Model;

		public void Awake()
		{
			if (Model == null)
			{
				Model = GetComponent<AvatarModel>();
			}
			Model.PartChanged += model_PartChanged;
			Model.OutfitSet += model_OutfitSet;
		}

		private void model_OutfitSet(IEnumerable<AvatarModel.ApplyResult> obj)
		{
			Crittercism.LeaveBreadcrumb(base.gameObject.name + " set an outfit");
		}

		private void model_PartChanged(int slotIndex, int partIndex, AvatarModel.Part oldPart, AvatarModel.Part newPart)
		{
			string name = Model.Definition.Slots[slotIndex].Name;
			string arg = AvatarDefinition.PartTypeStrings[partIndex];
			string key = string.Format("{0}.{1}.{2}", base.gameObject.name, name, arg);
			string value = (newPart != null && newPart.Equipment != null) ? newPart.Equipment.Name : string.Empty;
			Crittercism.SetValue(key, value);
		}

		public void OnDestroy()
		{
			Model.PartChanged -= model_PartChanged;
		}
	}
}
