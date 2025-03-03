namespace UnityEngine.UI.Extensions
{
	public class ReorderableListDebug : MonoBehaviour
	{
		public Text DebugLabel;

		private void Awake()
		{
			ReorderableList[] array = Object.FindObjectsOfType<ReorderableList>();
			foreach (ReorderableList reorderableList in array)
			{
				reorderableList.OnElementDropped.AddListener(ElementDropped);
			}
		}

		private void ElementDropped(ReorderableList.ReorderableListEventStruct droppedStruct)
		{
			DebugLabel.text = "";
			Text debugLabel = DebugLabel;
			debugLabel.text = debugLabel.text + "Dropped Object: " + droppedStruct.DroppedObject.name + "\n";
			Text debugLabel2 = DebugLabel;
			string text = debugLabel2.text;
			debugLabel2.text = text + "Is Clone ?: " + droppedStruct.IsAClone + "\n";
			if (droppedStruct.IsAClone)
			{
				Text debugLabel3 = DebugLabel;
				debugLabel3.text = debugLabel3.text + "Source Object: " + droppedStruct.SourceObject.name + "\n";
			}
			DebugLabel.text += string.Format("From {0} at Index {1} \n", droppedStruct.FromList.name, droppedStruct.FromIndex);
			DebugLabel.text += string.Format("To {0} at Index {1} \n", droppedStruct.ToList.name, droppedStruct.ToIndex);
		}
	}
}
