using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Fabric;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Holiday
{
	public abstract class InteractiveDecoration : MonoBehaviour
	{
		public string GroupName;

		public DecorationColor CurrentColor;

		public InteractiveDecorationSharedData SharedData;

		private MaterialPropertyBlock sharedBlock;

		private Dictionary<DecorationColor, Vector2> colorOffsetData;

		private DateTime currentContentDate;

		public virtual void Start()
		{
			currentContentDate = Service.Get<ContentSchedulerService>().ScheduledEventDate();
			if (SharedData != null)
			{
				if (!string.IsNullOrEmpty(GroupName))
				{
					sharedBlock = SharedData.GetSharedBlock(GroupName);
					colorOffsetData = SharedData.GetColorOffsetData(GroupName);
				}
				else
				{
					Log.LogError(this, string.Format("O_o\t Error: {0} has an empty block name", base.gameObject.GetPath()));
				}
			}
			else
			{
				Log.LogError(this, string.Format("O_o\t Error: {0} is missing it's SharedData controller", base.gameObject.GetPath()));
			}
		}

		public virtual void ChangeColor(Renderer rend, DecorationColor newColor)
		{
			Vector2 offset = new Vector2(0f, 0f);
			if (colorOffsetData.ContainsKey(newColor))
			{
				offset = colorOffsetData[newColor];
			}
			else
			{
				Log.LogError(this, string.Format("O_o\t {0}: Can't find color {1} in colorOffsetData", base.gameObject.GetPath(), newColor));
			}
			shiftMaterial(rend, offset);
		}

		private void shiftMaterial(Renderer rend, Vector2 offset)
		{
			if (rend != null && sharedBlock != null)
			{
				sharedBlock.SetVector("_MainTex_ST", new Vector4(1f, 1f, offset.x, offset.y));
				rend.SetPropertyBlock(sharedBlock);
			}
		}

		public virtual bool DuringHidePhase()
		{
			if (SharedData != null && currentContentDate.Date >= SharedData.HideStartDate.Date && currentContentDate.Date <= SharedData.HideEndDate.Date)
			{
				return true;
			}
			return false;
		}

		public virtual void OnColorChange()
		{
		}

		public virtual void GroupSetActive(bool isActive)
		{
			MeshRenderer component = base.gameObject.GetComponent<MeshRenderer>();
			if (component != null)
			{
				component.enabled = isActive;
			}
			foreach (Transform item in base.transform)
			{
				item.gameObject.SetActive(isActive);
			}
		}

		public virtual void PlayAudioEvent(string audioEventName, GameObject anchorObj = null)
		{
			if (!string.IsNullOrEmpty(audioEventName))
			{
				if (anchorObj != null)
				{
					EventManager.Instance.PostEvent(audioEventName, EventAction.PlaySound, anchorObj);
				}
				else
				{
					EventManager.Instance.PostEvent(audioEventName, EventAction.PlaySound);
				}
			}
		}
	}
}
