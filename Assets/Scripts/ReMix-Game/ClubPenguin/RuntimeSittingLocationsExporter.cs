using ClubPenguin.Actions;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin
{
	public class RuntimeSittingLocationsExporter
	{
		public IList<ExportedSittingLocation> ExportCurrentScene()
		{
			List<ExportedSittingLocation> list = new List<ExportedSittingLocation>();
			SetSitLocomotionAction[] array = Object.FindObjectsOfType<SetSitLocomotionAction>();
			foreach (SetSitLocomotionAction setSitLocomotionAction in array)
			{
				ExportedSittingLocation item = default(ExportedSittingLocation);
				item.position = new InWorldExportPosition(setSitLocomotionAction.gameObject.transform.position);
				item.gameObject = setSitLocomotionAction.gameObject.GetPath();
				list.Add(item);
			}
			list.Sort((ExportedSittingLocation x, ExportedSittingLocation y) => x.gameObject.CompareTo(y.gameObject));
			return list;
		}
	}
}
