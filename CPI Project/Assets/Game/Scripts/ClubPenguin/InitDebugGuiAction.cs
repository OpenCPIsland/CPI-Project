using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using Tweaker.Core;
using UnityEngine;

namespace ClubPenguin
{
	[RequireComponent(typeof(InitCoreServicesAction))]
	[RequireComponent(typeof(InitContentSystemAction))]
	[RequireComponent(typeof(InitGuiAction))]
	[RequireComponent(typeof(InitDiagnosticsAction))]
	public class InitDebugGuiAction : InitActionComponent
	{
		public bool PerformanceDisplay = true;

		public bool PrototypeDisplay = true;

		public bool ContentVersionDisplay = true;

		public override bool HasSecondPass
		{
			get
			{
				return false;
			}
		}

		public override bool HasCompletedPass
		{
			get
			{
				return false;
			}
		}

		[Tweakable("Debug.ShowDebugGUI")]
		public static bool ShowDebugGUI
		{
			get
			{
				return Service.Get<Canvas>().GetComponentInChildren<DebugGui>(true).gameObject.activeSelf;
			}
			set
			{
				Service.Get<Canvas>().GetComponentInChildren<DebugGui>(true).gameObject.SetActive(value);
			}
		}

		public override IEnumerator PerformFirstPass()
		{
			Canvas canvas = Service.Get<Canvas>();
			DebugGui componentInChildren = canvas.GetComponentInChildren<DebugGui>();
			if (componentInChildren != null)
			{
				PerformanceDisplay component = componentInChildren.GetComponent<PerformanceDisplay>();
				component.enabled = PerformanceDisplay;
				Service.Set(component);
				PrototypeDisplay component2 = componentInChildren.GetComponent<PrototypeDisplay>();
				ContentVersionDisplay component3 = componentInChildren.GetComponent<ContentVersionDisplay>();
				component3.enabled = ContentVersionDisplay;
				component2.enabled = PrototypeDisplay;
				Service.Set(componentInChildren);
			}
			yield break;
		}
	}
}
