using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace MixAvatar
{
	public class Equipment : MonoBehaviour
	{
		[NonSerialized]
		private EquipmentPart[] _parts = null;

		[HideInInspector]
		public List<EquipmentPart> EjectedParts = new List<EquipmentPart>();

		public bool AllPartsEjected { get; private set; }

		public EquipmentPart[] Parts
		{
			get
			{
				if (_parts == null)
				{
					_parts = new EquipmentPart[base.transform.childCount];
					for (int i = 0; i < base.transform.childCount; i++)
					{
						EquipmentPart component = base.transform.GetChild(i).GetComponent<EquipmentPart>();
						if (component == null)
						{
							throw new Exception("Equipment may only consist of EquipmentPart children.");
						}
						_parts[i] = component;
						component.EjectedFromSlot += onPartEjectedFromSlot;
					}
				}
				return _parts;
			}
		}

		private void onPartEjectedFromSlot(EquipmentPart equipmentPart)
		{
			if (!AllPartsEjected)
			{
				if (equipmentPart.EjectSiblingsOnOverlap)
				{
					EjectAllParts();
					return;
				}
				EjectedParts.Add(equipmentPart);
				AllPartsEjected = EjectedParts.Count == Parts.Length;
			}
		}

		public void ResetEjectionInfo()
		{
			EjectedParts.Clear();
			AllPartsEjected = false;
		}

		public void EjectAllParts()
		{
			EjectedParts.Clear();
			for (int i = 0; i < Parts.Length; i++)
			{
				Slot occupiedSlot = Parts[i].OccupiedSlot;
				if (occupiedSlot != null)
				{
					occupiedSlot.RemovePart(Parts[i]);
				}
				EjectedParts.Add(Parts[i]);
			}
			AllPartsEjected = true;
		}

		[Conditional("UNITY_EDITOR")]
		public void RebindPreCompiledShaders()
		{
			EquipmentPart[] parts = Parts;
			foreach (EquipmentPart equipmentPart in parts)
			{
				if (!(equipmentPart == null) && !(equipmentPart.SkinnedMeshRenderer == null) && !(equipmentPart.SkinnedMeshRenderer.sharedMaterial == null))
				{
					Shader shader = Shader.Find(equipmentPart.SkinnedMeshRenderer.sharedMaterial.shader.name);
					equipmentPart.SkinnedMeshRenderer.sharedMaterial.shader = shader;
				}
			}
		}

		public void OnValidate()
		{
		}
	}
}
