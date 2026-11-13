using System.Collections;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class VariableLayoutSetter : MonoBehaviour
	{
		[SerializeField]
		private string layoutID = string.Empty;

		private void OnEnable()
		{
			StartCoroutine(updateLayout());
		}

		private IEnumerator updateLayout()
		{
			LayoutMappings mappings = GetComponentInParent<LayoutMappings>();
			while (mappings == null)
			{
				yield return null;
				mappings = GetComponentInParent<LayoutMappings>();
			}
			string layoutType = mappings.GetLayoutType(layoutID);
			if (layoutType != null)
			{
				ILayoutSettingsSwitcher[] components = GetComponents<ILayoutSettingsSwitcher>();
				ILayoutSettingsSwitcher[] array = components;
				foreach (ILayoutSettingsSwitcher layoutSettingsSwitcher in array)
				{
					layoutSettingsSwitcher.ApplySettingsForLayout(layoutType);
				}
			}
		}
	}
}
