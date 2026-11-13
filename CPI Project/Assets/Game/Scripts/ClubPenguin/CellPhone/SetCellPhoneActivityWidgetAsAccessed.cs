using UnityEngine;

namespace ClubPenguin.CellPhone
{
	public class SetCellPhoneActivityWidgetAsAccessed : MonoBehaviour
	{
		public string WidgetIdentifier;

		private void Start()
		{
			setWidgetAsAccessed();
		}

		private void setWidgetAsAccessed()
		{
			CellPhoneActivityScreenDefinition.AccessedWidgets accessedWidgets = CellPhoneActivityScreenDefinition.AccessedWidgets.GetAccessedWidgets();
			if (!accessedWidgets.Widgets.Contains(WidgetIdentifier))
			{
				accessedWidgets.Widgets.Add(WidgetIdentifier);
				CellPhoneActivityScreenDefinition.AccessedWidgets.SaveAccessedWidgets(accessedWidgets);
			}
		}
	}
}
