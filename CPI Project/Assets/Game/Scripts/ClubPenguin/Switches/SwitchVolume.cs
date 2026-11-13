using ClubPenguin.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Switches
{
	[RequireComponent(typeof(Collider))]
	public class SwitchVolume : Switch
	{
		public Color EditorGizmoColor = Color.white;

		public string Tag = "Player";

		public void OnTriggerEnter(Collider col)
		{
			if (col.CompareTag(Tag))
			{
				Change(true);
			}
		}

		public void OnTriggerExit(Collider col)
		{
			if (col.CompareTag(Tag))
			{
				Change(false);
			}
		}

		public override string GetSwitchType()
		{
			return "volume";
		}

		public override object GetSwitchParameters()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			if (Tag != "Player")
			{
				throw new NotSupportedException("No server support to export a switch volume with the tag " + Tag + " in switch " + base.gameObject.GetPath());
			}
			List<Dictionary<string, object>> colliderBounds = ExportUtils.GetColliderBounds(GetComponents<Collider>());
			dictionary.Add("colliders", colliderBounds);
			return dictionary;
		}
	}
}
